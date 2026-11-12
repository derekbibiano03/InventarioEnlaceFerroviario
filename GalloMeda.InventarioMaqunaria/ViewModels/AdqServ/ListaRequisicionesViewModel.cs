using Azure.Identity;
using GalloMeda.InventarioMaqunaria;
using Inventario.Core.DTOs.Requisicion;
using Inventario.Core.Services.Adq_Serv.AdquisicionService;
using Inventario.Data.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Graph;
using Microsoft.Graph.Models;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Inventario.Desktop.ViewModels.AdqServ
{
    public class ListaRequisicionesViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public ObservableCollection<Usuario> Usuarios { get; } = new();

        private ObservableCollection<RequisicionListDto> _requisiciones = new();
        public ObservableCollection<RequisicionListDto> Requisiciones
        {
            get => _requisiciones;
            set { _requisiciones = value; OnPropertyChanged(); }
        }

        private RequisicionListDto? _requisicionSeleccionada;
        public RequisicionListDto? RequisicionSeleccionada
        {
            get => _requisicionSeleccionada;
            set { _requisicionSeleccionada = value; OnPropertyChanged(); }
        }

        public ICommand VerExcelCommand { get; }
        public ICommand AutorizarCommand { get; }
        public ICommand NoAutorizarCommand { get; }
        private readonly AdquisicionService _reqService;

        public ListaRequisicionesViewModel(AdquisicionService reqService)
        {
            _reqService = reqService ?? throw new ArgumentNullException(nameof(reqService));

            Usuarios = new ObservableCollection<Usuario>();
            VerExcelCommand = new RelayCommand(_ => AbrirArchivoExcel());
            AutorizarCommand = new RelayCommand(_ => AutorizarArchivo());
            NoAutorizarCommand = new RelayCommand(_ => NoAutorizarArchivo());
            _ = CargarRequisicionesAsync();
        }

        public async Task EnviarCorreoNotificacionConArchivoAsync(List<string> correosDestino, List<string> correosCc, string asunto, string cuerpoHtml, string rutaArchivo)
        {
            var configuration = new Microsoft.Extensions.Configuration.ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            string tenantId = configuration["AzureAd:TenantId"];
            string clientId = configuration["AzureAd:ClientId"];
            string clientSecret = configuration["AzureAd:ClientSecret"];
            string remitenteCorreo = configuration["AzureAd:Remitente"];

            var options = new ClientSecretCredentialOptions();

            var clientSecretCredential = new ClientSecretCredential(
                tenantId, clientId, clientSecret, options);

            var graphClient = new GraphServiceClient(clientSecretCredential, new[] { "https://graph.microsoft.com/.default" });

            byte[] fileBytes = File.ReadAllBytes(rutaArchivo);
            string fileName = Path.GetFileName(rutaArchivo);

            var toRecipientsList = correosDestino.Select(email => new Recipient
            {
                EmailAddress = new EmailAddress
                {
                    Address = email
                }
            }).ToList();

            var ccRecipientsList = correosCc.Select(email => new Recipient
            {
                EmailAddress = new EmailAddress
                {
                    Address = email
                }
            }).ToList();

            var requestBody = new Microsoft.Graph.Users.Item.SendMail.SendMailPostRequestBody
            {
                Message = new Message
                {
                    Subject = asunto,
                    Body = new ItemBody
                    {
                        ContentType = BodyType.Html,
                        Content = cuerpoHtml
                    },
                    ToRecipients = toRecipientsList,
                    CcRecipients = ccRecipientsList,
                    Attachments = new List<Attachment>
                    {
                        new FileAttachment
                        {
                            OdataType = "#microsoft.graph.fileAttachment",
                            Name = fileName,
                            ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                            ContentBytes = fileBytes
                        }
                    }
                },
                SaveToSentItems = true
            };

            await graphClient.Users[remitenteCorreo].SendMail.PostAsync(requestBody);
        }

        public async Task CargarRequisicionesAsync()
        {
            try
            {
                if (App.Session == null)
                {
                    MessageBox.Show("App.Session es nulo.", "Depuración");
                    return;
                }

                var datosReq = await _reqService.ObtenerRequisicionesAsync(App.Session.IdUsuario);

                Requisiciones.Clear();
                foreach (var dato in datosReq)
                {
                    Requisiciones.Add(dato);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"StackTrace: {ex.StackTrace}\n\nMensaje: {ex.Message}", "Depuración de Error Nulo", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void NoAutorizarArchivo()
        {
            if (RequisicionSeleccionada == null)
            {
                MessageBox.Show("Por favor, seleccione una requisición de la lista.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrEmpty(RequisicionSeleccionada.ArchivoReq))
            {
                MessageBox.Show("La requisición seleccionada no cuenta con un archivo adjunto.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                string firmaPathUsuario = App.Session?.FirmaPath ?? string.Empty;

                if (string.IsNullOrWhiteSpace(firmaPathUsuario))
                {
                    MessageBox.Show("El usuario actual no cuenta con una ruta de firma registrada en la sesión.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // 1. El servicio descarga, estampa la firma del autorizante y actualiza la BD
                string rutaTemporalExcelFirmado = await _reqService.NoAutorizarAsync(
                    RequisicionSeleccionada.IdRequisicion
                );
            }
            catch (Exception ex)
            {
                string mensajeReal = ex.InnerException?.Message ?? ex.Message;
                MessageBox.Show($"No se pudo autorizar el archivo: {mensajeReal}", "Error Crítico", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void AutorizarArchivo()
        {
            if (RequisicionSeleccionada == null)
            {
                MessageBox.Show("Por favor, seleccione una requisición de la lista.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrEmpty(RequisicionSeleccionada.ArchivoReq))
            {
                MessageBox.Show("La requisición seleccionada no cuenta con un archivo adjunto.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                string firmaPathUsuario = App.Session?.FirmaPath ?? string.Empty;

                if (string.IsNullOrWhiteSpace(firmaPathUsuario))
                {
                    MessageBox.Show("El usuario actual no cuenta con una ruta de firma registrada en la sesión.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // 1. El servicio descarga, estampa la firma del autorizante y actualiza la BD
                string rutaTemporalExcelFirmado = await _reqService.AutorizarYDescargarArchivoAsync(
                    RequisicionSeleccionada.IdRequisicion,
                    RequisicionSeleccionada.ArchivoReq,
                    firmaPathUsuario
                );

                if (string.IsNullOrEmpty(rutaTemporalExcelFirmado) || !File.Exists(rutaTemporalExcelFirmado))
                {
                    throw new Exception("El servicio no pudo generar o retornar el archivo firmado.");
                }

                // 2. BLOQUEO DE EDICIÓN POST-FIRMA: Aseguramos proteger las hojas de forma limpia
                using (var workbook = new ClosedXML.Excel.XLWorkbook(rutaTemporalExcelFirmado))
                {
                    foreach (var ws in workbook.Worksheets)
                    {
                        // Si la hoja ya estuviera protegida, la desprotegemos temporalmente para evitar excepciones
                        if (ws.IsProtected)
                        {
                            ws.Unprotect();
                        }

                        // Aplicamos la protección definitiva contra edición
                        ws.Protect("GMOX4568_MAQ159654");
                    }
                    workbook.Save();
                }

                string archivoFinalParaEnviar = rutaTemporalExcelFirmado;

                var saveFileDialog = new SaveFileDialog
                {
                    Filter = "Excel Files|*.xlsx",
                    FileName = RequisicionSeleccionada.ArchivoReq
                };

                if (saveFileDialog.ShowDialog() == true)
                {
                    File.Copy(rutaTemporalExcelFirmado, saveFileDialog.FileName, true);
                    archivoFinalParaEnviar = saveFileDialog.FileName;
                }

                // 3. Envío del correo electrónico de notificación
                var destinatarios = new List<string> { RequisicionSeleccionada.CorreoAtencion };
                var copias = new List<string> { "rrodriguez@enlaceferroviario.com", "egarcia@enlaceferroviario.com", RequisicionSeleccionada.CorreoSolicitante };

                await EnviarCorreoNotificacionConArchivoAsync(
                    correosDestino: destinatarios,
                    correosCc: copias,
                    asunto: $"Requisición {RequisicionSeleccionada.GradoPrioridad} Autorizada - {RequisicionSeleccionada.ArchivoReq}",
                    cuerpoHtml: $"<p>Hola Malcom,</p><p>La {RequisicionSeleccionada.TipoRequisicion} <b>{RequisicionSeleccionada.ArchivoReq}</b> ha sido autorizada. Se adjunta el formato de Excel firmado y bloqueado.</p>",
                    rutaArchivo: archivoFinalParaEnviar
                );

                // 4. Actualización de la lista en interfaz
                await CargarRequisicionesAsync();

                MessageBox.Show("Requisición autorizada, estatus cambiado a 'AUTORIZADA' y archivo enviado correctamente.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                string mensajeReal = ex.InnerException?.Message ?? ex.Message;
                MessageBox.Show($"No se pudo autorizar el archivo: {mensajeReal}", "Error Crítico", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void AbrirArchivoExcel()
        {
            try
            {
                if (RequisicionSeleccionada == null)
                {
                    MessageBox.Show("Por favor, seleccione una requisición de la lista.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (string.IsNullOrEmpty(RequisicionSeleccionada.ArchivoReq))
                {
                    MessageBox.Show("La requisición seleccionada no cuenta con un archivo adjunto.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                _reqService.DescargarYAbrirArchivoExcel(RequisicionSeleccionada.ArchivoReq);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"DETALLE DEL ERROR:\n\nMensaje: {ex.Message}\n\nStackTrace:\n{ex.StackTrace}",
                                "Error en AbrirArchivoExcel", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}