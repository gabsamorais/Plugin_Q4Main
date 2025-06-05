namespace tmrp.core
{
    using System.Collections.Generic;

    // Estrutura dos dados de entrada do formulário de dimensionamento de equipes
    public class DimEqpComandoDados
    {
        public string TipoServicoDE { get; set; }
        public bool TaxChegadaModelo { get; set; }
        public bool TaxChegadaElic { get; set; }
        public List<double> ListaTaxChegadaElic { get; set; }
        public bool TaxAtendModelo { get; set; }
        public bool TaxAtendElic { get; set; }
        public float PercentAtend { get; set; }
        public float CustoAtend { get; set; }
        public float CustoEsp { get; set; }

        public DimEqpComandoDados() { }
    }
}