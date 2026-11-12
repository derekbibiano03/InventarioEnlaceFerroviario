using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Inventario.Data.Models
{
    public class DetalleRequisicion : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        private int _partida;
        public int Partida { get => _partida; set { _partida = value; OnPropertyChanged(); } }

        private decimal _cantidad;
        public decimal Cantidad { get => _cantidad; set { _cantidad = value; OnPropertyChanged(); } }

        private string? _unidad;
        public string? Unidad { get => _unidad; set { _unidad = value; OnPropertyChanged(); } }

        private string? _descripcion;
        public string? Descripcion { get => _descripcion; set { _descripcion = value; OnPropertyChanged(); } }

        private string? _noPartida;
        public string? NoPartida { get => _noPartida; set { _noPartida = value; OnPropertyChanged(); } }

        private string? _noEquivalente;
        public string? NoEquivalente { get => _noEquivalente; set { _noEquivalente = value; OnPropertyChanged(); } }

        private string? _catalogo;
        public string? Catalogo { get => _catalogo; set { _catalogo = value; OnPropertyChanged(); } }

        private string? _pagina;
        public string? Pagina { get => _pagina; set { _pagina = value; OnPropertyChanged(); } }

        private string? _conjunto;
        public string? Conjunto { get => _conjunto; set { _conjunto = value; OnPropertyChanged(); } }

        private string? _subconjunto;
        public string? Subconjunto { get => _subconjunto; set { _subconjunto = value; OnPropertyChanged(); } }

        private string? _linea;
        public string? Linea { get => _linea; set { _linea = value; OnPropertyChanged(); } }
    }
}