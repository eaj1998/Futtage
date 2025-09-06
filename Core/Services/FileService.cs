using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Futtage.Core.Services
{
    public class FileService : IFileService
    {
        private readonly List<string> _tempFiles = new List<string>();

        public async Task<List<string>> SelectVideoFilesAsync()
        {
            System.Diagnostics.Debug.WriteLine("=== SelectVideoFilesAsync INICIADO ===");

            return await Task.Run(() =>
            {
                var result = new List<string>();

                try
                {
                    Application.OpenForms[0]?.Invoke(new Action(() =>
                    {
                        System.Diagnostics.Debug.WriteLine("Abrindo diálogo de seleção de arquivos...");

                        using var openFileDialog = new OpenFileDialog
                        {
                            Filter = "Arquivos de Vídeo MP4 (*.mp4)|*.mp4|Todos os arquivos (*.*)|*.*",
                            Multiselect = true,
                            Title = "Selecione os arquivos de vídeo"
                        };

                        System.Diagnostics.Debug.WriteLine("Mostrando diálogo...");
                        var dialogResult = openFileDialog.ShowDialog();
                        System.Diagnostics.Debug.WriteLine($"Resultado do diálogo: {dialogResult}");

                        if (dialogResult == DialogResult.OK)
                        {
                            result.AddRange(openFileDialog.FileNames);
                            System.Diagnostics.Debug.WriteLine($"Arquivos selecionados: {result.Count}");

                            foreach (var file in result)
                            {
                                System.Diagnostics.Debug.WriteLine($"- {file}");
                            }
                        }
                        else
                        {
                            System.Diagnostics.Debug.WriteLine("Usuário cancelou a seleção");
                        }
                    }));
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"ERRO na seleção de arquivos: {ex.Message}");
                }

                System.Diagnostics.Debug.WriteLine("=== SelectVideoFilesAsync FINALIZADO ===");
                return result;
            });
        }

        public async Task<string> SelectOutputPathAsync(string defaultName)
        {
            System.Diagnostics.Debug.WriteLine("=== SelectOutputPathAsync INICIADO ===");
            System.Diagnostics.Debug.WriteLine($"Nome padrão: {defaultName}");

            return await Task.Run(() =>
            {
                string result = string.Empty;

                try
                {
                    Application.OpenForms[0]?.Invoke(new Action(() =>
                    {
                        System.Diagnostics.Debug.WriteLine("Abrindo diálogo de salvar arquivo...");

                        using var saveFileDialog = new SaveFileDialog
                        {
                            Filter = "Arquivos de Vídeo MP4 (*.mp4)|*.mp4",
                            FileName = defaultName,
                            Title = "Salvar vídeo como..."
                        };

                        System.Diagnostics.Debug.WriteLine("Mostrando diálogo de salvar...");
                        var dialogResult = saveFileDialog.ShowDialog();
                        System.Diagnostics.Debug.WriteLine($"Resultado do diálogo de salvar: {dialogResult}");

                        if (dialogResult == DialogResult.OK)
                        {
                            result = saveFileDialog.FileName;
                            System.Diagnostics.Debug.WriteLine($"Caminho selecionado: {result}");
                        }
                        else
                        {
                            System.Diagnostics.Debug.WriteLine("Usuário cancelou o salvamento");
                        }
                    }));
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"ERRO no diálogo de salvar: {ex.Message}");
                }

                System.Diagnostics.Debug.WriteLine("=== SelectOutputPathAsync FINALIZADO ===");
                return result;
            });
        }

        public async Task<string> SelectImageFileAsync()
        {
            System.Diagnostics.Debug.WriteLine("=== SelectImageFileAsync INICIADO ===");

            return await Task.Run(() =>
            {
                string result = string.Empty;

                try
                {
                    Application.OpenForms[0]?.Invoke(new Action(() =>
                    {
                        using var openFileDialog = new OpenFileDialog
                        {
                            Filter = "Arquivos de Imagem|*.jpg;*.jpeg;*.png;*.bmp|Todos os arquivos (*.*)|*.*",
                            Title = "Selecione uma imagem para thumbnail"
                        };

                        if (openFileDialog.ShowDialog() == DialogResult.OK)
                        {
                            result = openFileDialog.FileName;
                            System.Diagnostics.Debug.WriteLine($"Imagem selecionada: {result}");
                        }
                    }));
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"ERRO na seleção de imagem: {ex.Message}");
                }

                return result;
            });
        }

        public string GetTempFilePath(string extension = ".tmp")
        {
            var tempPath = Path.Combine(Path.GetTempPath(), $"futtage_{Guid.NewGuid()}{extension}");
            _tempFiles.Add(tempPath);
            System.Diagnostics.Debug.WriteLine($"Arquivo temporário criado: {tempPath}");
            return tempPath;
        }

        public void DeleteTempFiles()
        {
            System.Diagnostics.Debug.WriteLine($"Removendo {_tempFiles.Count} arquivos temporários...");

            foreach (var file in _tempFiles.Where(File.Exists))
            {
                try
                {
                    File.Delete(file);
                    System.Diagnostics.Debug.WriteLine($"Removido: {file}");
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Erro ao remover {file}: {ex.Message}");
                }
            }
            _tempFiles.Clear();
        }

        public bool FileExists(string path)
        {
            var exists = File.Exists(path);
            System.Diagnostics.Debug.WriteLine($"FileExists({path}): {exists}");
            return exists;
        }

        public long GetFileSize(string path)
        {
            try
            {
                var size = new FileInfo(path).Length;
                System.Diagnostics.Debug.WriteLine($"FileSize({path}): {size} bytes");
                return size;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro ao obter tamanho de {path}: {ex.Message}");
                return 0;
            }
        }
    }
}