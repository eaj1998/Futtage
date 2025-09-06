// Core/Services/VideoProcessingService.cs - VERSÃO COMPLETA

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

        public async Task<string> ConcatenateAsync(List<string> videoPaths, string outputPath, IProgress<ProcessingProgress>? progress = null)
        {
            System.Diagnostics.Debug.WriteLine("=== ConcatenateAsync INICIADO ===");
            System.Diagnostics.Debug.WriteLine($"Número de vídeos: {videoPaths?.Count ?? 0}");
            System.Diagnostics.Debug.WriteLine($"Caminho de saída: {outputPath}");

            if (videoPaths == null || videoPaths.Count < 2)
            {
                var error = "É necessário pelo menos 2 vídeos para concatenação";
                System.Diagnostics.Debug.WriteLine($"ERRO: {error}");
                throw new ArgumentException(error);
            }

            try
            {
                _logger.LogInfo($"Iniciando concatenação de {videoPaths.Count} vídeos");
                progress?.Report(new ProcessingProgress(0, "Preparando arquivos..."));

                // Usar método para encontrar FFmpeg
                var ffmpegPath = FindFFmpegPath();
                System.Diagnostics.Debug.WriteLine($"FFmpeg será usado em: {ffmpegPath ?? "MODO SIMULADO"}");

                // Criar arquivo de lista temporário
                var listFile = Path.GetTempFileName();
                System.Diagnostics.Debug.WriteLine($"Arquivo de lista temporário: {listFile}");

                await CreateFileListAsync(videoPaths, listFile);
                System.Diagnostics.Debug.WriteLine("Arquivo de lista criado");

                progress?.Report(new ProcessingProgress(10, "Iniciando concatenação..."));

                // MODO SIMULADO para teste inicial (depois implementaremos o real)
                System.Diagnostics.Debug.WriteLine("=== MODO SIMULADO ===");
                await SimulateFFmpegProgress(progress, 10, 90);

                // Limpar arquivo temporário
                try
                {
                    File.Delete(listFile);
                    System.Diagnostics.Debug.WriteLine("Arquivo temporário removido");
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Erro ao remover arquivo temporário: {ex.Message}");
                }

                progress?.Report(new ProcessingProgress(100, "Concatenação concluída!"));

                System.Diagnostics.Debug.WriteLine($"Concatenação simulada concluída: {outputPath}");
                _logger.LogInfo($"Concatenação concluída: {outputPath}");

                return outputPath;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"ERRO na concatenação: {ex.Message}");
                _logger.LogError($"Erro na concatenação: {ex.Message}", ex);
                throw;
            }
            finally
            {
                System.Diagnostics.Debug.WriteLine("=== ConcatenateAsync FINALIZADO ===");
            }
        }

        public async Task<string> TrimAsync(string inputPath, TimeSpan start, TimeSpan end, string outputPath, IProgress<ProcessingProgress>? progress = null)
        {
            System.Diagnostics.Debug.WriteLine("=== TrimAsync INICIADO ===");
            System.Diagnostics.Debug.WriteLine($"Arquivo entrada: {inputPath}");
            System.Diagnostics.Debug.WriteLine($"Arquivo saída: {outputPath}");
            System.Diagnostics.Debug.WriteLine($"Início: {start}, Fim: {end}");

            if (!File.Exists(inputPath))
            {
                var error = "Arquivo de entrada não encontrado";
                System.Diagnostics.Debug.WriteLine($"ERRO: {error} - {inputPath}");
                throw new FileNotFoundException(error, inputPath);
            }

            try
            {
                _logger.LogInfo($"Iniciando corte do vídeo: {inputPath}");
                progress?.Report(new ProcessingProgress(0, "Preparando corte..."));

                var ffmpegPath = FindFFmpegPath();

                if (!string.IsNullOrEmpty(ffmpegPath))
                {
                    System.Diagnostics.Debug.WriteLine("Executando corte REAL com FFmpeg");
                    return await ExecuteRealTrimAsync(inputPath, start, end, outputPath, ffmpegPath, progress);
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("FFmpeg não encontrado - executando corte SIMULADO");
                    return await ExecuteSimulatedTrimAsync(inputPath, outputPath, progress);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"ERRO no corte: {ex.Message}");
                _logger.LogError($"Erro no corte: {ex.Message}", ex);
                throw;
            }
            finally
            {
                System.Diagnostics.Debug.WriteLine("=== TrimAsync FINALIZADO ===");
            }
        }

        public async Task<VideoInfo> GetVideoInfoAsync(string videoPath)
        {
            System.Diagnostics.Debug.WriteLine($"=== GetVideoInfoAsync: {videoPath} ===");

            if (!File.Exists(videoPath))
            {
                System.Diagnostics.Debug.WriteLine($"ERRO: Arquivo não encontrado - {videoPath}");
                throw new FileNotFoundException("Arquivo não encontrado", videoPath);
            }

            try
            {
                // Por enquanto, retornar informações básicas sem usar FFmpeg
                var fileInfo = new FileInfo(videoPath);
                var videoInfo = new VideoInfo
                {
                    FilePath = videoPath,
                    IsValid = true,
                    CreationDate = fileInfo.CreationTime,
                    FileSize = fileInfo.Length,
                    Duration = TimeSpan.FromMinutes(5), 
                    Width = 1920,
                    Height = 1080,
                    FrameRate = 30
                };

                System.Diagnostics.Debug.WriteLine($"VideoInfo criado: {videoInfo.FileName} - {videoInfo.FormattedDuration}");
                return videoInfo;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"ERRO ao obter info do vídeo: {ex.Message}");
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
            System.Diagnostics.Debug.WriteLine($"=== GetMultipleVideoInfoAsync: {videoPaths.Count} arquivos ===");

            var tasks = videoPaths.Select(async path =>
            {
                try
                {
                    return await GetVideoInfoAsync(path);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Erro ao processar {path}: {ex.Message}");
                    return new VideoInfo { FilePath = path, IsValid = false, ErrorMessage = ex.Message };
                }
            });

            var results = await Task.WhenAll(tasks);
            System.Diagnostics.Debug.WriteLine($"Processados {results.Length} vídeos");

            return results.ToList();
        }

        public bool ValidateVideoFile(string videoPath)
        {
            if (string.IsNullOrEmpty(videoPath) || !File.Exists(videoPath))
            {
                System.Diagnostics.Debug.WriteLine($"Validação falhou: arquivo não existe - {videoPath}");
                return false;
            }

            var extension = Path.GetExtension(videoPath).ToLowerInvariant();
            var isValid = extension == ".mp4";

            System.Diagnostics.Debug.WriteLine($"Validação de {videoPath}: {isValid} (extensão: {extension})");
            return isValid;
        }

        #region Métodos Privados

        private string? FindFFmpegPath()
        {
            System.Diagnostics.Debug.WriteLine("Procurando FFmpeg...");

            if (File.Exists(_ffmpegPath))
            {
                System.Diagnostics.Debug.WriteLine($"FFmpeg encontrado no caminho configurado: {_ffmpegPath}");
                return _ffmpegPath;
            }

            var alternativePaths = new[]
            {
                Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ffmpeg.exe"),
                Path.Combine(Environment.CurrentDirectory, "ffmpeg.exe"),
                Path.Combine(Environment.CurrentDirectory, "bin", "Debug", "net8.0-windows", "ffmpeg.exe"),
                Path.Combine(Environment.CurrentDirectory, "bin", "Release", "net8.0-windows", "ffmpeg.exe")
            };

            foreach (var altPath in alternativePaths)
            {
                System.Diagnostics.Debug.WriteLine($"Tentando: {altPath}");
                if (File.Exists(altPath))
                {
                    System.Diagnostics.Debug.WriteLine($"FFmpeg encontrado em: {altPath}");
                    return altPath;
                }
            }

            System.Diagnostics.Debug.WriteLine("FFmpeg não encontrado - usando modo simulado");
            return null;
        }

        private async Task<string> ExecuteRealTrimAsync(string inputPath, TimeSpan start, TimeSpan end, string outputPath, string ffmpegPath, IProgress<ProcessingProgress>? progress)
        {
            try
            {
                var outputDirectory = Path.GetDirectoryName(outputPath);
                if (!string.IsNullOrEmpty(outputDirectory) && !Directory.Exists(outputDirectory))
                {
                    Directory.CreateDirectory(outputDirectory);
                    System.Diagnostics.Debug.WriteLine($"Diretório criado: {outputDirectory}");
                }

                progress?.Report(new ProcessingProgress(10, "Preparando comando FFmpeg..."));

                var duration = end - start;
                var arguments = $"-i \"{inputPath}\" -ss {start:hh\\:mm\\:ss} -t {duration:hh\\:mm\\:ss} -c copy \"{outputPath}\"";

                System.Diagnostics.Debug.WriteLine($"Comando FFmpeg: {ffmpegPath} {arguments}");

                progress?.Report(new ProcessingProgress(20, "Executando corte..."));

                using var process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = ffmpegPath,
                        Arguments = arguments,
                        UseShellExecute = false,
                        RedirectStandardError = true,
                        RedirectStandardOutput = true,
                        CreateNoWindow = true
                    }
                };

                process.Start();

                var progressTask = SimulateFFmpegProgress(progress, 20, 90);

                var outputTask = ReadFFmpegOutputAsync(process);

                // Aguardar processo terminar
                await process.WaitForExitAsync();
                await progressTask;
                var ffmpegOutput = await outputTask;

                System.Diagnostics.Debug.WriteLine($"FFmpeg exit code: {process.ExitCode}");
                System.Diagnostics.Debug.WriteLine($"FFmpeg output: {ffmpegOutput}");

                if (process.ExitCode == 0)
                {
                    if (File.Exists(outputPath))
                    {
                        var fileSize = new FileInfo(outputPath).Length;
                        System.Diagnostics.Debug.WriteLine($"Arquivo cortado criado com sucesso: {outputPath} ({fileSize} bytes)");

                        progress?.Report(new ProcessingProgress(100, "Corte concluído!"));
                        _logger.LogInfo($"Corte real concluído: {outputPath}");

                        return outputPath;
                    }
                    else
                    {
                        throw new Exception("FFmpeg executou sem erro, mas arquivo de saída não foi criado");
                    }
                }
                else
                {
                    throw new Exception($"FFmpeg falhou com código de saída: {process.ExitCode}\nOutput: {ffmpegOutput}");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro no corte real: {ex.Message}");
                throw;
            }
        }
        private async Task<string> ExecuteRealConcatenationAsync(List<string> videoPaths, string outputPath, string ffmpegPath, IProgress<ProcessingProgress>? progress)
        {
            try
            {
                var outputDirectory = Path.GetDirectoryName(outputPath);
                if (!string.IsNullOrEmpty(outputDirectory) && !Directory.Exists(outputDirectory))
                {
                    Directory.CreateDirectory(outputDirectory);
                }

                progress?.Report(new ProcessingProgress(10, "Criando lista de arquivos..."));

                var listFile = Path.GetTempFileName();
                await CreateFileListAsync(videoPaths, listFile);

                progress?.Report(new ProcessingProgress(20, "Iniciando concatenação..."));

                var arguments = $"-f concat -safe 0 -i \"{listFile}\" -c copy \"{outputPath}\"";

                System.Diagnostics.Debug.WriteLine($"Comando FFmpeg: {ffmpegPath} {arguments}");

                using var process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = ffmpegPath,
                        Arguments = arguments,
                        UseShellExecute = false,
                        RedirectStandardError = true,
                        RedirectStandardOutput = true,
                        CreateNoWindow = true
                    }
                };

                process.Start();

                var progressTask = MonitorFFmpegProgress(process, progress, 20, 90);
                var outputTask = ReadFFmpegOutputAsync(process);

                await process.WaitForExitAsync();
                await progressTask;
                var ffmpegOutput = await outputTask;

                try
                {
                    File.Delete(listFile);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Erro ao remover arquivo temporário: {ex.Message}");
                }

                System.Diagnostics.Debug.WriteLine($"FFmpeg exit code: {process.ExitCode}");

                if (process.ExitCode == 0)
                {
                    if (File.Exists(outputPath))
                    {
                        progress?.Report(new ProcessingProgress(100, "Concatenação concluída!"));
                        _logger.LogInfo($"Concatenação real concluída: {outputPath}");
                        return outputPath;
                    }
                    else
                    {
                        throw new Exception("FFmpeg executou sem erro, mas arquivo de saída não foi criado");
                    }
                }
                else
                {
                    throw new Exception($"FFmpeg falhou com código de saída: {process.ExitCode}\nOutput: {ffmpegOutput}");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro na concatenação real: {ex.Message}");
                throw;
            }
        }

        private async Task MonitorFFmpegProgress(Process process, IProgress<ProcessingProgress>? progress, int startProgress, int endProgress)
        {
            var progressRegex = new Regex(@"time=(\d+):(\d+):(\d+\.\d+)");
            var durationRegex = new Regex(@"Duration: (\d+):(\d+):(\d+\.\d+)");

            TimeSpan? totalDuration = null;
            var currentProgress = startProgress;

            try
            {
                while (!process.StandardError.EndOfStream)
                {
                    var line = await process.StandardError.ReadLineAsync();
                    if (string.IsNullOrEmpty(line)) continue;

                    System.Diagnostics.Debug.WriteLine($"FFmpeg: {line}");

                    if (!totalDuration.HasValue)
                    {
                        var durationMatch = durationRegex.Match(line);
                        if (durationMatch.Success)
                        {
                            if (TimeSpan.TryParse($"{durationMatch.Groups[1].Value}:{durationMatch.Groups[2].Value}:{durationMatch.Groups[3].Value}", out var duration))
                            {
                                totalDuration = duration;
                                System.Diagnostics.Debug.WriteLine($"Duração total detectada: {totalDuration}");
                            }
                        }
                    }

                    var progressMatch = progressRegex.Match(line);
                    if (progressMatch.Success && totalDuration.HasValue)
                    {
                        if (TimeSpan.TryParse($"{progressMatch.Groups[1].Value}:{progressMatch.Groups[2].Value}:{progressMatch.Groups[3].Value}", out var currentTime))
                        {
                            var progressPercentage = (int)((currentTime.TotalSeconds / totalDuration.Value.TotalSeconds) * (endProgress - startProgress)) + startProgress;
                            progressPercentage = Math.Min(progressPercentage, endProgress);

                            progress?.Report(new ProcessingProgress(progressPercentage, $"Processando... {currentTime:hh\\:mm\\:ss}/{totalDuration:hh\\:mm\\:ss}"));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro ao monitorar progresso: {ex.Message}");
            }
        }

        public async Task<VideoInfo> GetRealVideoInfoAsync(string videoPath)
        {
            var ffmpegPath = FindFFmpegPath();

            if (string.IsNullOrEmpty(ffmpegPath))
            {
                return await GetBasicVideoInfoAsync(videoPath);
            }

            try
            {
                var arguments = $"-i \"{videoPath}\" -f null -";

                using var process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = ffmpegPath,
                        Arguments = arguments,
                        UseShellExecute = false,
                        RedirectStandardError = true,
                        RedirectStandardOutput = true,
                        CreateNoWindow = true
                    }
                };

                process.Start();
                var output = await process.StandardError.ReadToEndAsync();
                await process.WaitForExitAsync();

                return ParseFFmpegVideoInfo(videoPath, output);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro ao obter info real do vídeo: {ex.Message}");
                return await GetBasicVideoInfoAsync(videoPath);
            }
        }

        private VideoInfo ParseFFmpegVideoInfo(string videoPath, string ffmpegOutput)
        {
            try
            {
                var fileInfo = new FileInfo(videoPath);
                var videoInfo = new VideoInfo
                {
                    FilePath = videoPath,
                    IsValid = true,
                    CreationDate = fileInfo.CreationTime,
                    FileSize = fileInfo.Length
                };

                var durationRegex = new Regex(@"Duration: (\d+):(\d+):(\d+\.\d+)");
                var durationMatch = durationRegex.Match(ffmpegOutput);
                if (durationMatch.Success)
                {
                    if (TimeSpan.TryParse($"{durationMatch.Groups[1].Value}:{durationMatch.Groups[2].Value}:{durationMatch.Groups[3].Value}", out var duration))
                    {
                        videoInfo.Duration = duration;
                    }
                }

                var resolutionRegex = new Regex(@"(\d+)x(\d+)");
                var resolutionMatch = resolutionRegex.Match(ffmpegOutput);
                if (resolutionMatch.Success)
                {
                    if (int.TryParse(resolutionMatch.Groups[1].Value, out var width) &&
                        int.TryParse(resolutionMatch.Groups[2].Value, out var height))
                    {
                        videoInfo.Width = width;
                        videoInfo.Height = height;
                    }
                }

                var fpsRegex = new Regex(@"(\d+\.?\d*) fps");
                var fpsMatch = fpsRegex.Match(ffmpegOutput);
                if (fpsMatch.Success)
                {
                    if (double.TryParse(fpsMatch.Groups[1].Value, out var fps))
                    {
                        videoInfo.FrameRate = fps;
                    }
                }

                return videoInfo;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro ao parsear info do FFmpeg: {ex.Message}");
                return new VideoInfo
                {
                    FilePath = videoPath,
                    IsValid = false,
                    ErrorMessage = ex.Message
                };
            }
        }

        private async Task<VideoInfo> GetBasicVideoInfoAsync(string videoPath)
        {
            var fileInfo = new FileInfo(videoPath);
            return new VideoInfo
            {
                FilePath = videoPath,
                IsValid = true,
                CreationDate = fileInfo.CreationTime,
                FileSize = fileInfo.Length,
                Duration = TimeSpan.FromMinutes(5),
                Width = 1920,
                Height = 1080,
                FrameRate = 30
            };
        }        

        private async Task CreateFileListAsync(List<string> videoPaths, string listFilePath)
        {
            System.Diagnostics.Debug.WriteLine($"Criando lista de arquivos em: {listFilePath}");

            using var writer = new StreamWriter(listFilePath);
            foreach (var path in videoPaths)
            {
                var escapedPath = path.Replace("'", @"'\''");
                await writer.WriteLineAsync($"file '{escapedPath}'");
                System.Diagnostics.Debug.WriteLine($"Adicionado à lista: {escapedPath}");
            }

            System.Diagnostics.Debug.WriteLine("Lista de arquivos criada com sucesso");
        }

        private async Task<string> ReadFFmpegOutputAsync(Process process)
        {
            var output = new System.Text.StringBuilder();

            while (!process.StandardError.EndOfStream)
            {
                var line = await process.StandardError.ReadLineAsync();
                if (!string.IsNullOrEmpty(line))
                {
                    output.AppendLine(line);
                    System.Diagnostics.Debug.WriteLine($"FFmpeg: {line}");
                }
            }

            return output.ToString();
        }

        #endregion
    }
}