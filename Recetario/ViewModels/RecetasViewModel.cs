using Microsoft.Maui.Controls.Shapes;
using Recetario.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Recetario.ViewModels
{
    public enum ElegirLista { Todas, Favoritos }
    public class RecetasViewModel : INotifyPropertyChanged
    {
        ElegirLista listalistas;

        public ObservableCollection<RecetaModel> RecetasLista { get; set; } = new ObservableCollection<RecetaModel>();
        public List<RecetaModel> Favoritos { get; set; } = new List<RecetaModel>();
        public List<RecetaModel> ListaRecetas { get; set; } = new List<RecetaModel>();
        public IEnumerable<ElegirLista> ListaElegida => Enum.GetValues(typeof(ElegirLista)).Cast<ElegirLista>();
        public ElegirLista ListaListas { get => listalistas; set { listalistas = value; FiltrarRecetas(); } }

        private RecetaModel receta;
        public RecetaModel Receta
        {
            get { return receta; }
            set
            {
                receta = value;
                PropertyChanged?.Invoke(this, new(nameof(Receta)));
            }
        }

        public ICommand EntrarCommand { get; set; }
        public ICommand OpcionesCommand { get; set; }
        public ICommand AgregarFavoritosCommand { get; set; }
        public ICommand VerRecetaCommand { get; set; }
        public ICommand PantallaPrincipalCommand { get; set; }
        public ICommand EliminarFavoritosCommand { get; set; }
        public ICommand EliminarTodosLosFavoritosCommand { get; set; }

        private string searchText;
        public string SearchText
        {
            get { return searchText; }
            set
            {
                if (searchText != value)
                {
                    searchText = value;
                    PropertyChanged?.Invoke(this, new(nameof(SearchText)));
                    FiltrarRecetas();
                }
            }
        }

        public RecetasViewModel()
        {
            EntrarCommand = new Command(Entrar);
            OpcionesCommand = new Command(Opciones);
            AgregarFavoritosCommand = new Command<RecetaModel>(AgregarFavoritos);
            VerRecetaCommand = new Command<RecetaModel>(VerReceta);
            PantallaPrincipalCommand = new Command(Pantallaprincipal);
            EliminarFavoritosCommand = new Command<RecetaModel>(EliminarFavoritos);
            EliminarTodosLosFavoritosCommand = new Command(EliminarTodosLosFavoritos);

            Deserializar();
            Actualizar();
            Cargar();
        }

        private void EliminarTodosLosFavoritos()
        {
            Favoritos.Clear();
            PropertyChanged?.Invoke(this, new(nameof(Favoritos)));
            FiltrarRecetas();
            Shell.Current.GoToAsync("//ListaRecetas");
            Guardar();
        }

        private void EliminarFavoritos(RecetaModel receta)
        {
            if (receta != null)
            {
                Receta = receta;
                Favoritos.Remove(Receta);
                PropertyChanged?.Invoke(this, new(nameof(Favoritos)));
                FiltrarRecetas();
                Guardar();
            }
        }

        private void Opciones()
        {
            Shell.Current.GoToAsync("//Opciones");
        }
        private void Pantallaprincipal()
        {
            Shell.Current.GoToAsync("//Presentacion");
        }

        private void VerReceta(RecetaModel receta)
        {
            Receta = receta;
            PropertyChanged?.Invoke(this, new(nameof(Receta)));
            Shell.Current.GoToAsync("//Receta");
        }

        private void AgregarFavoritos(RecetaModel receta)
        {
            if (receta != null)
            {
                Receta = receta;
                if (!Favoritos.Contains(receta))
                {
                    Favoritos.Add(receta);
                    PropertyChanged?.Invoke(this, new(nameof(Favoritos)));
                    Guardar();
                }
            }
        }
        private void Entrar()
        {
            Shell.Current.GoToAsync("//ListaRecetas");
        }
        private void Deserializar()
        {
            Stream s = FileSystem.OpenAppPackageFileAsync("Recetas.json").Result;
            var lista = JsonSerializer.Deserialize<List<RecetaModel>>(s);
            if(lista != null )
            {
                ListaRecetas.AddRange(lista);
            }
        }
        private void FiltrarRecetas()
        {
            RecetasLista.Clear();
            IEnumerable<RecetaModel> recetasFiltradas;

            if (string.IsNullOrWhiteSpace(SearchText))
            {
                if (ListaListas == ElegirLista.Todas)
                {
                    recetasFiltradas = ListaRecetas;
                }
                else
                {
                    recetasFiltradas = Favoritos;
                }
            }
            else
            {
                recetasFiltradas = ListaRecetas.Where(receta => receta.nombre.ToLower().Contains(SearchText.ToLower()));
            }

            foreach (var receta in recetasFiltradas.OrderBy(x => x.nombre))
            {
                RecetasLista.Add(receta);
            }
        }
        private void Actualizar()
        {
            for(int i = 0; i < ListaRecetas.Count; i++)
            {
                var item = ListaRecetas[i];
                RecetasLista.Add(item);
            }
        }
        private void Guardar()
        {
            var ruta = FileSystem.AppDataDirectory;
            File.WriteAllText(ruta + "/Favoritos.json", JsonSerializer.Serialize(Favoritos));
        }
        private void Cargar()
        {
            var ruta = FileSystem.AppDataDirectory;
            if (File.Exists(ruta + "/Favoritos.json"))
            {
                var datos = JsonSerializer.Deserialize<List<RecetaModel>>(File.ReadAllText(ruta + "/Favoritos.json"));
                if (datos != null)
                {
                    Favoritos.AddRange(datos);
                }
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
