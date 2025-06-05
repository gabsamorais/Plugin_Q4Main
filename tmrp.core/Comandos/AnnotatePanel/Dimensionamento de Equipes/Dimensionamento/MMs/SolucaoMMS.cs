namespace tmrp.core
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Text.Json;
    using System.Globalization;
    using System.Collections.Generic;
    using MathNet.Numerics.Optimization;
    using Autodesk.Revit.DB;

    // Implementa toda a lógica de cálculo de um modelo de filas M/M/s
    public class FilaMMS
    {
        // Retorna a função de distribuição acumulada (cdf) de uma V.A. exponencial
        static double funcExp(double x, double a)
        {
            return 1 - Math.Exp(-a * x);
        }

        // Retorna uma lista contendo o valor dos parâmetros de uma distribuição exponencial ajustada aos dados elicitados [lambda, erro padrão, média, variância]
        // numChamadosElic é uma lista com os valores elicitados junto ao especialista para os números de chamados
        // As probabilidades correspondentes ao intevalo plausível e percentis dos valores elicitados são [0.001, 0.125, 0.25, 0.375, 0.50, 0.625, 0.75, 0.875, 0.99]
        // O método usado para otimização é o Nelder Mead Simplex (Biblioteca MathNet.Numerics)
        public static double[] AjusteTmc(double[] numChamadosElic)
        {
            double[] probabilidades = { 0.001, 0.125, 0.25, 0.375, 0.50, 0.625, 0.75, 0.875, 0.99 };
                        
            Func<MathNet.Numerics.LinearAlgebra.Vector<double>, double> erroAjuste= (MathNet.Numerics.LinearAlgebra.Vector<double> parametros) =>
            {
                double lambda = parametros[0];
                double erro = 0;
                for (int i = 0; i < numChamadosElic.Length; i++)
                {
                    double previsto = funcExp(numChamadosElic[i], lambda);
                    erro += Math.Pow(previsto - probabilidades[i], 2);
                }
                
                return erro;
            };

            var palpiteInicial = MathNet.Numerics.LinearAlgebra.Vector<double>.Build.DenseOfArray(new double[] { 0.1 });
            var resultado = NelderMeadSimplex.Minimum(ObjectiveFunction.Value(erroAjuste), palpiteInicial);
            double lambdaOtimo = resultado.MinimizingPoint[0];
            double medLamb = 1 / lambdaOtimo;
            double varLamb = 1 / (lambdaOtimo * lambdaOtimo);
            double erroPadrao = numChamadosElic.Select(x => Math.Pow(funcExp(x, lambdaOtimo) - probabilidades.Average(), 2)).Average();
            erroPadrao = Math.Sqrt(erroPadrao);

            return new double[] { lambdaOtimo, erroPadrao, medLamb, varLamb };
        }

        // Retorna a taxa de utilização para a instalação de atendimento
        public static double FatorUtilizacao(double taxaChegada, double taxaAtendimento, int numeroAtendentes)
        {
            return taxaChegada / (taxaAtendimento * numeroAtendentes);
        }

        // Retorna o número mínimo de atendentes para alcançar a condição de estado estável (p < 1)
        public static int NumMinAtendentes(double taxaChegada, double taxaAtendimento)
        {
            return (int)Math.Ceiling(taxaChegada / (taxaAtendimento * 0.99));
        }

        // Retorna a probabilidade não haver clientes no sistema de fila (P0)
        // soma, parcela2 e parcela3 são parcelas que compõem o cálculo da probabilidade
        // sup e inf correspondem ao intervalo do somatório (superior e inferior, respectivamente)
        public static double ProbabilidadeZeroClientes(int numeroAtendentes, double taxaChegada, double taxaAtendimento)
        {
            double soma = 0;
            int sup = numeroAtendentes - 1;
            int inf = 0;

            while (inf <= sup)
            {
                soma += Math.Pow(taxaChegada / taxaAtendimento, inf) / Fatorial(inf);
                inf++;
            }

            double parcela2 = Math.Pow(taxaChegada / taxaAtendimento, numeroAtendentes) / Fatorial(numeroAtendentes);
            double parcela3 = 1 / (1 - taxaChegada / (taxaAtendimento * numeroAtendentes));

            return 1 / (soma + parcela2 * parcela3);
        }

        // Retorna o fatorial de um número
        private static int Fatorial(int n)
        {
            if (n == 0 || n == 1)
                return 1;
            else
                return n * Fatorial(n - 1);
        }

        //Retorna a probabilidade haver 'n' clientes no sistema de fila(Pn)
        public static double ProbabilidadeNClientes(int numeroAtendentes, int numeroClientes, double taxaChegada, double taxaAtendimento)
        {
            double probZero = ProbabilidadeZeroClientes(numeroAtendentes, taxaChegada, taxaAtendimento);

            double probN;

            if (numeroClientes > numeroAtendentes)
            {
                double p1 = Math.Pow(taxaChegada / taxaAtendimento, numeroClientes);
                double p2 = Fatorial(numeroAtendentes);
                double p3 = Math.Pow(numeroAtendentes, numeroClientes - numeroAtendentes);
                probN = (p1 / (p2 * p3)) * probZero;
            }
            else
            {
                double p1 = Math.Pow(taxaChegada / taxaAtendimento, numeroClientes);
                double p2 = Fatorial(numeroClientes);
                probN = (p1 / p2) * probZero;
            }

            return probN;
        }

        // Retorna uma lista com as respectivas probabilidades de ocorrência de 1 a 25 clientes no sistema
        public static List<double> GerandoProbNClientes(int numeroAtendentes, double taxaChegada, double taxaAtendimento)
        {
            var probsClientes = new List<double>();

            for (int i = 0; i <= 25; i++)
            {
                double probClienteN = ProbabilidadeNClientes(numeroAtendentes, i, taxaChegada, taxaAtendimento);
                probsClientes.Add(probClienteN);
            }
            return probsClientes;
        }

        // Retorna o numero de clientes na fila (Lq)
        public static double NumeroClientesFila(double taxaChegada, double taxaAtendimento, int numeroAtendentes)
        {
            double lq = (ProbabilidadeZeroClientes(numeroAtendentes, taxaChegada, taxaAtendimento) *
                            Math.Pow(taxaChegada / taxaAtendimento, numeroAtendentes) *
                                FatorUtilizacao(taxaChegada, taxaAtendimento, numeroAtendentes)) /
                                    (Fatorial(numeroAtendentes) * Math.Pow(1 - FatorUtilizacao(taxaChegada, taxaAtendimento, numeroAtendentes), 2));
            return lq;
        }

        // Retorna o numero de clientes no sistema (L)
        public static double NumeroClientesSistema(double taxaChegada, double taxaAtendimento, int numeroAtendentes)
        {
            double l = NumeroClientesFila(taxaChegada, taxaAtendimento, numeroAtendentes) + (taxaChegada / taxaAtendimento);

            return l;
        }

        // Retorna o tempo de espera na fila (Wq)
        public static double TempoEsperaFila(double taxaChegada, double taxaAtendimento, int numeroAtendentes)
        {
            double wq = NumeroClientesFila(taxaChegada, taxaAtendimento, numeroAtendentes) / taxaChegada;

            return wq;
        }

        // Retorna o tempo de espera no sistema (W)
        public static double TempoEsperaSistema(double taxaChegada, double taxaAtendimento, int numeroAtendentes)
        {
            double w = TempoEsperaFila(taxaChegada, taxaAtendimento, numeroAtendentes) + (1 / taxaAtendimento);

            return w;
        }

        // Retorna a probabilidade de que o tempo de espera no sistema seja maior que 1 (W > 1)
        public static double EsperaSistemaMaior1Dia(int numeroAtendentes, double taxaChegada, double taxaAtendimento, double tempo)
        {
            double parte1 = Math.Exp(-taxaAtendimento * tempo);
            double parte2 = ProbabilidadeZeroClientes(numeroAtendentes, taxaChegada, taxaAtendimento);
            double parte3 = Math.Pow(taxaChegada / taxaAtendimento, numeroAtendentes);
            double parte4 = Fatorial(numeroAtendentes);
            double parte5 = 1 - FatorUtilizacao(taxaChegada, taxaAtendimento, numeroAtendentes);
            double parte6 = 1 - Math.Exp((-taxaAtendimento * tempo) * (numeroAtendentes - 1 - taxaChegada / taxaAtendimento));
            double parte7 = numeroAtendentes - 1 - taxaChegada / taxaAtendimento;

            double pwt = parte1 * (1 + ((parte2 * parte3) / (parte4 * parte5)) * (parte6 / parte7));

            return pwt;
        }

        // Retorna a probabilidade de que o tempo de espera na fila seja maior que 1 (Wq > 1)
        public static double EsperaFilaMaior1Dia(int numeroAtendentes, double taxaChegada, double taxaAtendimento, double tempo)
        {
            double somaProbN = 0;
            double probZero = ProbabilidadeZeroClientes(numeroAtendentes, taxaChegada, taxaAtendimento);

            for (int k = 0; k < numeroAtendentes; k++)
            {
                double p1 = Math.Pow(taxaChegada / taxaAtendimento, k);
                double p2 = Fatorial(k);
                somaProbN += (p1 / p2) * probZero;
            }

            double partq1 = 1 - somaProbN;
            double partq2 = Math.Exp((-numeroAtendentes * taxaAtendimento) * (1 - FatorUtilizacao(taxaChegada, taxaAtendimento, numeroAtendentes)) * tempo);

            double pwqt = partq1 * partq2;

            return pwqt;
        }

        // Retorna o custo do serviço (CS) para 'n' atendentes
        public static double CustoTotalServico(int numeroAtendentes, double custoAtendentes)
        {
            double cs = Math.Round(numeroAtendentes * custoAtendentes, 2);
            return cs;
        }

        // Retorna o custo da espera (CW) associada a ocorrência de 'n' clientes na fila
        public static double CustoTotalEspera(double numeroClientesSistemaCw, double custoEspera)
        {
            double cw = Math.Round(numeroClientesSistemaCw * custoEspera, 2);
            return cw;
        }

        // Retorna o custo total (CT), representado pela soma do custo do serviço (CS) mais o custo da espera (CW)
        public static double CustoTotal(int numeroAtendentes, double custoAtendentes, double numeroClientesSistemaCt, double custoEspera)
        {
            double ct = Math.Round(CustoTotalServico(numeroAtendentes, custoAtendentes) + CustoTotalEspera(numeroClientesSistemaCt, custoEspera), 2);
            return ct;
        }

        // Retorna uma lista com três listas representando respectivamente CS, CW e CT
        public static List<List<double>> GerandoListaCustos(List<int> numeroAtendentes, double custoAtendentes, double custoEspera, double taxaChegada, double taxaAtendimento)
        {
            List<List<double>> listaCustos = new List<List<double>>();
            List<double> listaCustosCs = new List<double>();
            List<double> listaCustosCw = new List<double>();
            List<double> listaCustosCt = new List<double>();

            foreach (int n in numeroAtendentes)
            {
                listaCustosCs.Add(CustoTotalServico(n, custoAtendentes));
                listaCustosCw.Add(CustoTotalEspera(NumeroClientesSistema(taxaChegada, taxaAtendimento, n), custoEspera));
                listaCustosCt.Add(CustoTotal(n, custoAtendentes, NumeroClientesSistema(taxaChegada, taxaAtendimento, n), custoEspera));
            }

            listaCustos.Add(listaCustosCs);
            listaCustos.Add(listaCustosCw);
            listaCustos.Add(listaCustosCt);

            return listaCustos;
        }

        // Retorna o número ótimo de atendentes levando em consideração CS, CW e CT
        public static int OtimizacaoAtendentes(double taxaChegada, double taxaAtendimento, double custoAtendentes, double custoEspera)
        {
            int numeroAtendentes = NumMinAtendentes(taxaChegada, taxaAtendimento);
            double numeroClientesS = NumeroClientesSistema(taxaChegada, taxaAtendimento, numeroAtendentes);
            double custoTotalInit = CustoTotal(numeroAtendentes, custoAtendentes, numeroClientesS, custoEspera);

            numeroAtendentes = numeroAtendentes + 1;
            numeroClientesS = NumeroClientesSistema(taxaChegada, taxaAtendimento, numeroAtendentes);
            double custoTotalFin = CustoTotal(numeroAtendentes, custoAtendentes, numeroClientesS, custoEspera);

            while (custoTotalFin < custoTotalInit)
            {
                numeroAtendentes = numeroAtendentes + 1;
                numeroClientesS = NumeroClientesSistema(taxaChegada, taxaAtendimento, numeroAtendentes);
                custoTotalInit = custoTotalFin;
                custoTotalFin = CustoTotal(numeroAtendentes, custoAtendentes, numeroClientesS, custoEspera);
            }

            numeroAtendentes = numeroAtendentes - 1;

            return numeroAtendentes;
        }

        // Retorna a taxa média de chegadas com base nos registros de manutenção do modelo
        public static double CalcularTaxCheg(string tipoServico, Document doc)
        {
            string nomeProjeto = Path.GetFileNameWithoutExtension(doc.PathName);
            string nomeArquivo = $"RegistroManutencao_{nomeProjeto}.json";
            string diretorio = Path.Combine(@"C:\Users\gabri\OneDrive - Universidade Federal de Pernambuco", nomeArquivo);
            if (!File.Exists(diretorio))
            {
                return 0;
            }
            string json = File.ReadAllText(diretorio);
            var manutenDadosCTC = JsonSerializer.Deserialize<List<RegManDados>>(json);
            if (manutenDadosCTC == null || manutenDadosCTC.Count == 0)
            {
                return 0;
            }

            var manutenDadosFiltCTC = manutenDadosCTC.Where(m => m.TipoServico.Equals(tipoServico, StringComparison.InvariantCultureIgnoreCase)).ToList();

            if (manutenDadosFiltCTC.Count < 2)
            {
                return 0;
            }

            var datasAbert = manutenDadosFiltCTC.Select(m => DateTime.ParseExact(m.DataAbertura, "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture).Date).OrderBy(d => d).ToList();
            
            List<double> intervalosTC = new List<double>();
            
            for (int i = 1; i < datasAbert.Count; i++)
            {
                double difDatasDias = (datasAbert[i] - datasAbert[i - 1]).TotalDays;
                intervalosTC.Add(difDatasDias);
            }

            double mediaChegadas = intervalosTC.Average();
            double lambCTC = 1 / mediaChegadas;
            return lambCTC;
        }

        // Retorna a taxa média de atendimento com base nos registros de manutenção do modelo
        public static double CalcularTaxAtend(string tipoServico, Document doc)
        {
            string nomeProjeto = Path.GetFileNameWithoutExtension(doc.PathName);
            string nomeArquivo = $"RegistroManutencao_{nomeProjeto}.json";
            string diretorio = Path.Combine(@"C:\Users\gabri\OneDrive - Universidade Federal de Pernambuco", nomeArquivo);
            if (!File.Exists(diretorio))
            {
                return 0;
            }
            string json = File.ReadAllText(diretorio);
            var manutenDadosTA = JsonSerializer.Deserialize<List<RegManDados>>(json);
            if (manutenDadosTA == null || manutenDadosTA.Count == 0)
            {
                return 0;
            }
            var manutenDadosFiltTA = manutenDadosTA.Where(m => m.TipoServico.Equals(tipoServico, StringComparison.InvariantCultureIgnoreCase)).ToList();

            if (manutenDadosFiltTA.Count == 0)
            {
                return 0;
            }

            List<double> intervalosTA = new List<double>();

            foreach (var registro in manutenDadosFiltTA)
            {
                DateTime dataAb = DateTime.ParseExact(registro.DataAbertura, "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture).Date;
                DateTime dataFc = DateTime.ParseExact(registro.DataFechamento, "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture).Date;
                double tempAtend = (dataFc - dataAb).TotalDays;
                intervalosTA.Add(tempAtend);
            }

            double mediaAtendimento = intervalosTA.Average();
            double miCTA = 1 / mediaAtendimento;
            return miCTA;
        }


        // Retorna o relatório com os dados obtidos da otimização do número de atendentes por meio do sistema de filas
        public static string GerarRelatorio(string tipoServico, double taxaChegada, double taxaAtendimento, double custoAtendentes, double custoEspera)
        {
            // Calculo dos parâmetros
            int nOtimoAtendentes = OtimizacaoAtendentes(taxaChegada, taxaAtendimento, custoAtendentes, custoEspera);
            double fatorUtilizacao = Math.Round(FatorUtilizacao(taxaChegada, taxaAtendimento, nOtimoAtendentes), 2);
            double espClienteFila = Math.Round(NumeroClientesFila(taxaChegada, taxaAtendimento, nOtimoAtendentes), 2);
            double espClienteSist = Math.Round(NumeroClientesSistema(taxaChegada, taxaAtendimento, nOtimoAtendentes), 2);
            double espTempoFila = Math.Round(TempoEsperaFila(taxaChegada, taxaAtendimento, nOtimoAtendentes), 2);
            double espTempoSist = Math.Round(TempoEsperaSistema(taxaChegada, taxaAtendimento, nOtimoAtendentes), 2);
            double probAtendenteOcioso = Math.Round(ProbabilidadeZeroClientes(nOtimoAtendentes, taxaChegada, taxaAtendimento), 2);
            double probW = EsperaSistemaMaior1Dia(nOtimoAtendentes, taxaChegada, taxaAtendimento, 1);
            double probWq = EsperaFilaMaior1Dia(nOtimoAtendentes, taxaChegada, taxaAtendimento, 1);
            double custoServicoFinal = CustoTotalServico(nOtimoAtendentes, custoAtendentes);
            double custoEsperaFinal = CustoTotalEspera(NumeroClientesSistema(taxaChegada, taxaAtendimento, nOtimoAtendentes), custoEspera);
            double custoTotalFinal = CustoTotal(nOtimoAtendentes, custoAtendentes, NumeroClientesSistema(taxaChegada, taxaAtendimento, nOtimoAtendentes), custoEspera);

            // Impressão dos resultados
            var relatorio = new StringBuilder();
            relatorio.AppendLine("\nServiço em análise: " + tipoServico + "\n");
            relatorio.AppendLine("Taxa de chegada (λ) = " + taxaChegada.ToString("F2") + " chegadas/dia\n");
            relatorio.AppendLine("Taxa de atendimento (μ) = " + taxaAtendimento.ToString("F2") + " atendimentos/dia\n");
            relatorio.AppendLine("Nº ideal de atendentes = " + nOtimoAtendentes + " equipes\n");
            relatorio.AppendLine("Fator de utilização (p) = " + fatorUtilizacao + "\n");
            relatorio.AppendLine("Comprimento esperado da fila (Lq) = " + espClienteFila + "\n");
            relatorio.AppendLine("Número de clientes esperado no sistema (L) = " + espClienteSist + "\n");
            relatorio.AppendLine("Tempo de espera na fila (Wq) = " + espTempoFila + "\n");
            relatorio.AppendLine("Tempo de espera no sistema (W) = " + espTempoSist + "\n");
            relatorio.AppendLine("Probabilidade de ociosidade (P0) = " + probAtendenteOcioso + "\n");
            relatorio.AppendLine("Custo do serviço (CS) = " + custoServicoFinal + "\n");
            relatorio.AppendLine("Custo da espera (CW) = " + custoEsperaFinal + "\n"   );
            relatorio.AppendLine("Custo total (CT) = " + custoTotalFinal + "\n");
            relatorio.AppendLine("P(W > 1) = " + probW + "\n");
            relatorio.AppendLine("P(Wq > 1) = " + probWq);

            return relatorio.ToString();
        }
    }
}