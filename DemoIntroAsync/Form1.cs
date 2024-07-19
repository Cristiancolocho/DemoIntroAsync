using System.Diagnostics;
using System.Security.Policy;

namespace DemoIntroAsync
{
    public partial class Form1 : Form
    {

        HttpClient httpClient = new HttpClient();

        public Form1()
        {
            InitializeComponent();
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            pictureBox1.Visible = true;

            var directorioActual = AppDomain.CurrentDomain.BaseDirectory;
            var destinoBaseSecuencial = Path.Combine(directorioActual, @"Imagenes\resultado-secuencial");
            var destinoBaseParalelo = Path.Combine(directorioActual, @"Imagenes\resultado-paralelo");
            PrepararEjecuci�n(destinoBaseParalelo, destinoBaseSecuencial);

            Console.WriteLine("inicio");
            List<Imagen> imagenes = ObtenerImagenes();

            //Parte secuencial

            var sw = new Stopwatch();
            sw.Start();

            foreach (var imagen in imagenes)
            {
                await ProcesarImagen(destinoBaseSecuencial, imagen);
            }

            Console.WriteLine("Secuencial - duraci�n en segundos: {0}",
                sw.ElapsedMilliseconds / 1000.0);

            sw.Reset();

            sw.Start();

            var tareasEnumerable = imagenes.Select(async imagen =>
            {
                await ProcesarImagen(destinoBaseParalelo, imagen);
            });

            await Task.WhenAll(tareasEnumerable);

            Console.WriteLine("Paralelo - duraci�n en segundos: {0}",
                sw.ElapsedMilliseconds / 1000.0);

            sw.Stop();
            

            pictureBox1.Visible = false;

        }

        private async Task ProcesarImagen(string directorio, Imagen imagen)
        {
            var respuesta = await httpClient.GetAsync(imagen.URL);
            var contenido = await respuesta.Content.ReadAsByteArrayAsync();

            Bitmap bitmap;
            using (var ms = new MemoryStream(contenido))
            {
                bitmap = new Bitmap(ms);
            }
            bitmap.RotateFlip(RotateFlipType.Rotate90FlipNone);
            var destino = Path.Combine(directorio, imagen.Nombre);
            bitmap.Save(destino);
        }

        private  static List<Imagen> ObtenerImagenes()
        {
            var imagenes = new List<Imagen>();

            for (int i = 0; i < 4; i++)
            {
                imagenes.Add(
                    new Imagen()
                    {
                        Nombre = $"Godzilla {i}.png",
                        URL = "https://cdn.autobild.es/sites/navi.axelspringer.es/public/media/image/2023/04/nissan-skyline-gt-r-r32-coche-electrico-3003884.jpg?tf=640x"
                    });
                imagenes.Add(
                    new Imagen()
                    {
                        Nombre = $"Super GT {i}.png",
                        URL = "https://upload.wikimedia.org/wikipedia/commons/thumb/0/05/Osaka_Auto_Messe_2014_%284%29_Nissan_GT-R_NISMO_GT500.JPG/220px-Osaka_Auto_Messe_2014_%284%29_Nissan_GT-R_NISMO_GT500.JPG"
                    });
                imagenes.Add(
                    new Imagen()
                    {
                        Nombre = $"GF-BNR34 {i}.png",
                        URL = "https://upload.wikimedia.org/wikipedia/commons/thumb/a/ac/Nissan_Skyline_R34_GT-R_N%C3%BCr_002.jpg/220px-Nissan_Skyline_R34_GT-R_N%C3%BCr_002.jpg"
                    });
            }
            return imagenes;
        }

        private void BorrarArchivos(string directorio)
        {
            var archivos =Directory.EnumerateFiles(directorio);
            foreach (var archivo in archivos)
            {
                File.Delete(archivo);
            }
        }

        private void PrepararEjecuci�n(string destinoBaseParalelo,
            string destinoBaseSecuencial)
        {
            if (!Directory.Exists(destinoBaseParalelo))
            {
                Directory.CreateDirectory(destinoBaseParalelo);
            }
            if (!Directory.Exists(destinoBaseSecuencial))
            {
                Directory.CreateDirectory(destinoBaseSecuencial);
            }

            BorrarArchivos(destinoBaseSecuencial);
            BorrarArchivos(destinoBaseParalelo);
        }

        private async Task<string> ProcesamientoLargo()
        {
            await Task.Delay(3000); //As�ncrono
            return "Felipe";
        }

        private async Task RealizarProcesamientoLargoA()
        {
            await Task.Delay(1000); //Asincrona
            Console.WriteLine("Proceso A finalizado");
        }

        private async Task RealizarProcesamientoLargoB()
        {
            await Task.Delay(1000); //Asincrona
            Console.WriteLine("Proceso B finalizado");
        }

        private async Task RealizarProcesamientoLargoC()
        {
            await Task.Delay(1000); //Asincrona
            Console.WriteLine("Proceso C finalizado");
        }

    }
}