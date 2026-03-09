// Core/Services/VideoProcessingService.cs - VERSÃO CORRIGIDA COMPLETA
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Futtage.Core.Models;
using Futtage.Infrastructure.Logging;

namespace Futtage.Core.Services
{
    public class VideoProcessingService : IVideoProcessingService
    {
        private readonly ILogger _logger;
        private string _ffmpegPath;

        public VideoProcessingService(ILogger logger, string ffmpegPath = "ffmpeg.exe")
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _ffmpegPath = ffmpegPath;
        }

        public async Task<string> ConcatenateAsync(List<string> videoPaths, string outputPath, IProgress<ProcessingProgress>? progress = null, CancellationToken cancellationToken = default)
        {
            if (videoPaths == null || videoPaths.Count < 2)
                throw new ArgumentException("É necessário pelo menos 2 vídeos para concatenação");

            var ffmpegPath = FindFFmpegPath();
            if (string.IsNullOrEmpty(ffmpegPath))
                throw new FileNotFoundException("FFmpeg não encontrado. Baixe e coloque ffmpeg.exe na pasta do aplicativo.");

            try
            {
                _logger.LogInfo($"Iniciando concatenação real de {videoPaths.Count} vídeos");
                progress?.Report(new ProcessingProgress(0, "Preparando concatenação..."));

                // Criar arquivo de lista temporário
                var listFile = Path.GetTempFileName();
                await CreateFileListAsync(videoPaths, listFile);

                //string listFile = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "mylist.txt");
                //await CreateFileListAsync(videoPaths, listFile);

                progress?.Report(new ProcessingProgress(10, "Iniciando concatenação..."));

                // Garantir que o diretório de saída existe
                var outputDir = Path.GetDirectoryName(outputPath);
                if (!string.IsNullOrEmpty(outputDir) && !Directory.Exists(outputDir))
                {
                    Directory.CreateDirectory(outputDir);
                }                

                // Comando FFmpeg para concatenação
                var arguments = $"-y -f concat -safe 0 -i \"{listFile}\" -c copy \"{outputPath}\"";

                try
                {
                    var videoInfos = await GetMultipleVideoInfoAsync(videoPaths);
                    var expectedDuration = TimeSpan.FromTicks(videoInfos.Sum(v => v.Duration.Ticks));

                    var result = await ExecuteFFmpegAsync(ffmpegPath, arguments, progress, 10, 90, false, cancellationToken, expectedDuration);
                    if (!result.Success)
                        throw new Exception($"FFmpeg falhou: {result.ErrorOutput}");
                }
                finally
                {
                    // Limpar arquivo temporário
                    try { File.Delete(listFile); } catch { }
                    
                    if (cancellationToken.IsCancellationRequested)
                    {
                        try { if (File.Exists(outputPath)) File.Delete(outputPath); } catch { }
                    }
                }

                cancellationToken.ThrowIfCancellationRequested();

                if (!File.Exists(outputPath))
                    throw new Exception("Arquivo de saída não foi criado");

                progress?.Report(new ProcessingProgress(100, "Concatenação concluída!"));
                _logger.LogInfo($"Concatenação concluída: {outputPath}");

                return outputPath;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Erro na concatenação: {ex.Message}", ex);
                throw;
            }
        }

        public async Task<string> TrimAsync(string inputPath, TimeSpan start, TimeSpan end, string outputPath, IProgress<ProcessingProgress>? progress = null, CancellationToken cancellationToken = default)
        {
            if (!File.Exists(inputPath))
                throw new FileNotFoundException("Arquivo de entrada não encontrado", inputPath);

            var ffmpegPath = FindFFmpegPath();
            if (string.IsNullOrEmpty(ffmpegPath))
                throw new FileNotFoundException("FFmpeg não encontrado. Baixe e coloque ffmpeg.exe na pasta do aplicativo.");

            try
            {
                _logger.LogInfo($"Iniciando corte real do vídeo: {inputPath}");
                progress?.Report(new ProcessingProgress(0, "Preparando corte..."));

                // Garantir que o diretório de saída existe
                var outputDir = Path.GetDirectoryName(outputPath);
                if (!string.IsNullOrEmpty(outputDir) && !Directory.Exists(outputDir))
                {
                    Directory.CreateDirectory(outputDir);
                }

                var duration = end - start;

                // Comando FFmpeg para corte - usando copy para ser mais rápido
                var arguments = $"-i \"{inputPath}\" -ss {start:hh\\:mm\\:ss\\.fff} -t {duration:hh\\:mm\\:ss\\.fff} -c copy \"{outputPath}\"";

                progress?.Report(new ProcessingProgress(10, "Iniciando corte..."));

                try
                {
                    var result = await ExecuteFFmpegAsync(ffmpegPath, arguments, progress, 10, 90, false, cancellationToken, duration);
                    
                    if (!result.Success)
                        throw new Exception($"FFmpeg falhou: {result.ErrorOutput}");
                }
                finally
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        try { if (File.Exists(outputPath)) File.Delete(outputPath); } catch { }
                    }
                }
                
                cancellationToken.ThrowIfCancellationRequested();

                if (!File.Exists(outputPath))
                    throw new Exception("Arquivo cortado não foi criado");

                progress?.Report(new ProcessingProgress(100, "Corte concluído!"));
                _logger.LogInfo($"Corte concluído: {outputPath}");

                return outputPath;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Erro no corte: {ex.Message}", ex);
                throw;
            }
        }

        public async Task<VideoInfo> GetVideoInfoAsync(string videoPath)
        {
            if (!File.Exists(videoPath))
                throw new FileNotFoundException("Arquivo não encontrado", videoPath);

            var ffmpegPath = FindFFmpegPath();
            if (string.IsNullOrEmpty(ffmpegPath))
            {
                // Fallback para informações básicas se FFmpeg não estiver disponível
                return GetBasicVideoInfo(videoPath);
            }

            try
            {
                // Usar ffprobe para obter informações detalhadas
                var ffprobePath = ffmpegPath.Replace("ffmpeg.exe", "ffprobe.exe");
                if (!File.Exists(ffprobePath))
                    ffprobePath = ffmpegPath; // Tentar com ffmpeg mesmo

                var arguments = $"-v quiet -print_format json -show_format -show_streams \"{videoPath}\"";

                var result = await ExecuteFFmpegAsync(ffprobePath, arguments, null, 0, 100, true);

                if (result.Success && !string.IsNullOrEmpty(result.Output))
                {
                    return ParseFFprobeOutput(videoPath, result.Output);
                }
                else
                {
                    // Fallback para método básico
                    return GetBasicVideoInfo(videoPath);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Erro ao obter informações do vídeo: {ex.Message}", ex);
                return new VideoInfo
                {
                    FilePath = videoPath,
                    IsValid = false,
                    ErrorMessage = ex.Message,
                    CreationDate = File.GetCreationTime(videoPath),
                    FileSize = new FileInfo(videoPath).Length
                };
            }
        }

        public async Task<List<VideoInfo>> GetMultipleVideoInfoAsync(List<string> videoPaths)
        {
            var tasks = videoPaths.Select(async path =>
            {
                try
                {
                    return await GetVideoInfoAsync(path);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning($"Erro ao processar {path}: {ex.Message}");
                    return new VideoInfo
                    {
                        FilePath = path,
                        IsValid = false,
                        ErrorMessage = ex.Message,
                        CreationDate = File.Exists(path) ? File.GetCreationTime(path) : DateTime.Now,
                        FileSize = File.Exists(path) ? new FileInfo(path).Length : 0
                    };
                }
            });

            return (await Task.WhenAll(tasks)).ToList();
        }

        public bool ValidateVideoFile(string videoPath)
        {
            if (string.IsNullOrEmpty(videoPath) || !File.Exists(videoPath))
                return false;

            try
            {
                var extension = Path.GetExtension(videoPath).ToLowerInvariant();
                var validExtensions = new[] { ".mp4", ".avi", ".mov", ".mkv", ".wmv", ".flv", ".webm" };

                if (!validExtensions.Contains(extension))
                    return false;

                // Verificar se arquivo não está vazio
                var fileInfo = new FileInfo(videoPath);
                return fileInfo.Length > 1024; // Pelo menos 1KB
            }
            catch
            {
                return false;
            }
        }

        #region Métodos Privados

        private string? FindFFmpegPath()
        {
            var possiblePaths = new[]
            {
                _ffmpegPath,
                Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ffmpeg.exe"),
                Path.Combine(Environment.CurrentDirectory, "ffmpeg.exe"),
                Path.Combine(Environment.CurrentDirectory, "tools", "ffmpeg.exe"),
                "ffmpeg" // Tentar no PATH do sistema
            };

            foreach (var path in possiblePaths)
            {
                try
                {
                    if (File.Exists(path))
                        return path;
                }
                catch { }
            }

            // Tentar encontrar no PATH
            try
            {
                var result = Process.Start(new ProcessStartInfo
                {
                    FileName = "where",
                    Arguments = "ffmpeg",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                });

                if (result != null)
                {
                    var output = result.StandardOutput.ReadToEnd();
                    result.WaitForExit();

                    if (result.ExitCode == 0 && !string.IsNullOrWhiteSpace(output))
                    {
                        var ffmpegPath = output.Split('\n')[0].Trim();
                        if (File.Exists(ffmpegPath))
                            return ffmpegPath;
                    }
                }
            }
            catch { }

            return null;
        }

        private async Task<(bool Success, string Output, string ErrorOutput)> ExecuteFFmpegAsync(
            string ffmpegPath,
            string arguments,
            IProgress<ProcessingProgress>? progress,
            int startProgress = 0,
            int endProgress = 100,
            bool captureOutput = false,
            CancellationToken cancellationToken = default,
            TimeSpan? expectedDuration = null)
        {
            using var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = ffmpegPath,
                    Arguments = arguments,
                    UseShellExecute = false,
                    RedirectStandardError = true,
                    RedirectStandardOutput = captureOutput,
                    CreateNoWindow = true,
                    WorkingDirectory = Path.GetDirectoryName(ffmpegPath) ?? Environment.CurrentDirectory
                }
            };

            var output = "";
            var errorOutput = "";

            process.Start();

            // Capturar output se necessário
            var outputTask = captureOutput ?
                ReadStreamAsync(process.StandardOutput, s => output += s, cancellationToken) :
                Task.CompletedTask;

            // Monitorar progresso através do stderr
            var progressTask = MonitorProgressAsync(process.StandardError, progress, startProgress, endProgress, s => errorOutput += s, cancellationToken, expectedDuration);

            try
            {
                await Task.WhenAll(outputTask, progressTask).WaitAsync(cancellationToken);
                await process.WaitForExitAsync(cancellationToken);
            }
            catch (OperationCanceledException)
            {
                if (!process.HasExited)
                {
                    process.Kill();
                }
                throw;
            }

            return (process.ExitCode == 0, output, errorOutput);
        }

        private async Task ReadStreamAsync(StreamReader reader, Action<string> onLineRead, CancellationToken cancellationToken)
        {
            string? line;
            while ((line = await reader.ReadLineAsync(cancellationToken)) != null)
            {
                cancellationToken.ThrowIfCancellationRequested();
                onLineRead(line + Environment.NewLine);
            }
        }

        private async Task MonitorProgressAsync(
            StreamReader errorStream,
            IProgress<ProcessingProgress>? progress,
            int startProgress,
            int endProgress,
            Action<string> onLineRead,
            CancellationToken cancellationToken,
            TimeSpan? expectedDuration = null)
        {
            var durationRegex = new Regex(@"Duration: (\d+):(\d+):(\d+\.\d+)");
            var progressRegex = new Regex(@"time=(\d+):(\d+):(\d+\.\d+)");

            TimeSpan? totalDuration = expectedDuration;
            string? line;

            while ((line = await errorStream.ReadLineAsync(cancellationToken)) != null)
            {
                cancellationToken.ThrowIfCancellationRequested();
                onLineRead(line + Environment.NewLine);

                if (progress == null) continue;

                // Tentar extrair duração total
                if (!totalDuration.HasValue)
                {
                    var durationMatch = durationRegex.Match(line);
                    if (durationMatch.Success)
                    {
                        if (TimeSpan.TryParse($"{durationMatch.Groups[1].Value}:{durationMatch.Groups[2].Value}:{durationMatch.Groups[3].Value}", out var duration))
                        {
                            totalDuration = duration;
                        }
                    }
                }

                // Extrair progresso atual
                var progressMatch = progressRegex.Match(line);
                if (progressMatch.Success && totalDuration.HasValue && totalDuration.Value.TotalSeconds > 0)
                {
                    if (TimeSpan.TryParse($"{progressMatch.Groups[1].Value}:{progressMatch.Groups[2].Value}:{progressMatch.Groups[3].Value}", out var currentTime))
                    {
                        var progressPercentage = (int)((currentTime.TotalSeconds / totalDuration.Value.TotalSeconds) * (endProgress - startProgress)) + startProgress;
                        progressPercentage = Math.Min(progressPercentage, endProgress);

                        var timeLeft = totalDuration.Value - currentTime;
                        var message = $"Processando... {currentTime:hh\\:mm\\:ss}/{totalDuration:hh\\:mm\\:ss} (resta: {timeLeft:hh\\:mm\\:ss})";

                        progress.Report(new ProcessingProgress(progressPercentage, message));
                    }
                }
            }
        }

        //private async Task CreateFileListAsync(List<string> videoPaths, string listFilePath)
        //{
        //    using var writer = new StreamWriter(listFilePath, false, System.Text.Encoding.UTF8);
        //    foreach (var path in videoPaths)
        //    {
        //        var escapedPath = path.Replace("'", @"'\''");
        //        await writer.WriteLineAsync($"file '{escapedPath}'");
        //    }

        //    await writer.FlushAsync();
        //}

       private Task CreateFileListAsync(List<string> videoPaths, string listFilePath)
        {
            var listaDeArquivos = new System.Text.StringBuilder();

            foreach (var path in videoPaths)
            {
                // Escape single quotes for FFmpeg concat format (e.g. John's -> John'\''s)
                var escapedPath = path.Replace("'", @"'\''");
                // Convert backslashes to forward slashes
                escapedPath = escapedPath.Replace("\\", "/");

                listaDeArquivos.AppendLine($"file '{escapedPath}'");
            }
            
            // Usar UTF-8 sem BOM (Byte Order Mark) porque FFmpeg é rígido quanto a textos BOM
            System.IO.File.WriteAllText(listFilePath, listaDeArquivos.ToString(), new System.Text.UTF8Encoding(false));
            
            return Task.CompletedTask;
        }     

        private VideoInfo ParseFFprobeOutput(string videoPath, string jsonOutput)
        {
            try
            {
                // Parse básico do JSON do ffprobe
                var fileInfo = new FileInfo(videoPath);
                var videoInfo = new VideoInfo
                {
                    FilePath = videoPath,
                    IsValid = true,
                    CreationDate = fileInfo.CreationTime,
                    FileSize = fileInfo.Length
                };

                // Extrair duração
                var durationMatch = Regex.Match(jsonOutput, @"""duration""\s*:\s*""([^""]+)""");
                if (durationMatch.Success && double.TryParse(durationMatch.Groups[1].Value, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var durationSeconds))
                {
                    videoInfo.Duration = TimeSpan.FromSeconds(durationSeconds);
                }

                // Extrair resolução
                var widthMatch = Regex.Match(jsonOutput, @"""width""\s*:\s*(\d+)");
                var heightMatch = Regex.Match(jsonOutput, @"""height""\s*:\s*(\d+)");

                if (widthMatch.Success && heightMatch.Success)
                {
                    videoInfo.Width = int.Parse(widthMatch.Groups[1].Value);
                    videoInfo.Height = int.Parse(heightMatch.Groups[1].Value);
                }

                // Extrair frame rate
                var fpsMatch = Regex.Match(jsonOutput, @"""r_frame_rate""\s*:\s*""(\d+)/(\d+)""");
                if (fpsMatch.Success)
                {
                    var num = double.Parse(fpsMatch.Groups[1].Value);
                    var den = double.Parse(fpsMatch.Groups[2].Value);
                    videoInfo.FrameRate = den > 0 ? num / den : 30;
                }

                return videoInfo;
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"Erro ao parsear saída do ffprobe: {ex.Message}");
                return GetBasicVideoInfo(videoPath);
            }
        }

        private VideoInfo GetBasicVideoInfo(string videoPath)
        {
            var fileInfo = new FileInfo(videoPath);
            return new VideoInfo
            {
                FilePath = videoPath,
                IsValid = true,
                CreationDate = fileInfo.CreationTime,
                FileSize = fileInfo.Length,
                Duration = TimeSpan.Zero, // Será mostrado como "desconhecido"
                Width = 1920, // Valores padrão
                Height = 1080,
                FrameRate = 30
            };
        }

        #endregion
    }
}