using ConsoleTables;
using gln;
using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;

namespace cqTimeSchool2 {
    public class Calcolo {

        public SWeek[] Weeks { get; set; }        
        private SClasse[] Classi { get; set; } = new SClasse[0];
        private SProfessore[] _professori { get; set; } = new SProfessore[0];
        public int NDay { get; set; }

        public Calcolo(int nDay, WeekDays[] configWeeks, params SProfessore[] prof) {
            NDay = nDay;            
            Weeks = SWeek.GenerateWeekEmpty(configWeeks, nDay);
            _professori = prof
                .OrderByDescending(x => x.Classi.Sum(c => c.OreFrontali))
                .ToArray();

            CalcClassi();
        }

        public void Start() {

            if (CheckComplete()) return;

            CalcAll();
            CalcError();

            DebugTableClasse();
            DebugTableProfessori();

            Console.WriteLine($"SOLVED => [{CheckComplete()}]");
            Console.WriteLine();

            var wClasse = ExcelClass.GetWorkBookTab("classi", GetDataTableClasse());
            var wProfess = ExcelClass.GetWorkBookTab("professori", GetDataTableProfessori());

            wClasse.Worksheets[0].AutoFitColumns();
            wProfess.Worksheets[0].AutoFitColumns();
            try {
                ExcelClass.SaveExcel(true, "calcolo.xls", wClasse, wProfess);
            } catch(Exception ex) {
                Console.WriteLine(ex);
            }
        }

        private void CalcAll() {
            if (CheckComplete()) return;
            foreach (var p in _professori)
                foreach (var c in p.Classi)
                    Calc(p, c, error: true);
        }

        private void CalcError() {            
            if (CheckComplete()) return;

            foreach (SWeek w in Weeks) {
                
                int retry = 0;
                CalcError:
                if (retry > NDay) continue;
                retry++;

                if (CheckComplete(w)) continue;
                ErrPC[] errors = w.GetCPError();
                if (errors.Count() == 0) continue;

                foreach (ErrPC e in errors) {
                    SClasse[] classi = e.Classi;
                    SClasse c = classi.First();

                    RisolviErr(w, e, c);
                }

                goto CalcError;
            }
        }

        private void RisolviErr(SWeek w, ErrPC e, SClasse c) {
            //trova gli errori di questa settimana in questa classe che non abbia questo index e questo professore
            SolutionP s = w.FoundSolutionCP(e.Index, e.Prof, c)
                //tutta questa manfrona per trovare quello più vicino a te                
                .OrderByDescending(x => x.Delta)
                .First();

            var pto = s.Profs.Last();
            w.Ore[e.Index].RemoveCP(c, e.Prof);
            w.Ore[s.Index].RemoveCP(c, pto);
            w.Ore[s.Index].AddCP(c, e.Prof);
            w.Ore[e.Index].AddCP(c, pto);
            
            /*
            Console.WriteLine($"[{w.Nome}{e.Index + 1}][{c}][{e.Prof}=>{pto}]--------------------");
            DebugLine("-", w.Nome, e.Index, c, e.Prof);
            DebugLine("-", w.Nome, sol.Index, c, pto);
            DebugLine("+", w.Nome, sol.Index, c, e.Prof);
            DebugLine("+", w.Nome, e.Index, c, pto);
            Console.WriteLine("-------------------------------------");
            */
        }

        private void DebugLine(string mode, WeekDays w, int index, SClasse c, SProfessore p) {
            Console.WriteLine($"{mode} [{w}{index + 1}] {c} {p} ");
        }

        private bool CheckComplete(SOra o) {
            return o.CountCP() == Classi.Count();
        }

        private bool CheckComplete(SWeek w) {
            return w.Ore.Count(x => CheckComplete(x)) == w.Ore.Count();
        }

        private bool CheckComplete() {
            return Weeks.Count(x => CheckComplete(x)) == Weeks.Count();
        }

        private SProfessore[] GetProfByClasse(SClasse c) {
            return _professori.Where(x => x.Classi.Count(z => $"{z}" == $"{c}") > 0).ToArray();
        }

        #region CALCOLI

        private void Calc(SProfessore p, SClasse c, bool error = false, int index = 0) {
            if (index >= NDay) return;

            foreach (SWeek w in Weeks) {
                CalcWeek(p, c, w, error, index);
            }

            Calc(p, c, error, ++index);
        }

        private void CalcWeek(SProfessore p, SClasse c, SWeek w, bool error, int index) {
            c = p.GetClasse(c);

            if (c == null) return;
            //ore libere prof
            var oFree = GetDiffOreErr(c, p);
            if (oFree >= 0) return;

            var o = w.Ore[index];
            var res = o.AddCP(c, p, error); // permette 
            //Console.WriteLine(res);
        }

        #endregion




        #region SERVIZI

        /// <summary>
        /// conta anche gli errori di sovrapposizione
        /// </summary>
        /// <param name="c"></param>
        /// <param name="p"></param>
        /// <returns></returns>
        public int GetDiffOreErr(SClasse c, SProfessore p) {
            c = p.GetClasse(c);
            if (c == null) return 0;
            return GetOreErr(c, p) - c.OreFrontali;
        }

        /// <summary>
        /// conta anche gli errori di sovrapposizione
        /// </summary>
        /// <param name="c"></param>
        /// <param name="p"></param>
        /// <returns></returns>
        public int GetOreErr(SClasse c, SProfessore p) {
            return this.Weeks.Sum(w => w.Ore.Count(o => o.CheckCPErr(c, p)));
        }


        /// <summary>
        /// non conta gli errori di sovrapposizione
        /// </summary>
        /// <param name="c"></param>
        /// <param name="p"></param>
        /// <returns></returns>
        public int GetDiffOre(SClasse c, SProfessore p) {
            c = p.GetClasse(c);
            if (c == null) return 0;
            return GetOre(c, p) - c.OreFrontali;
        }

        public int GetOre(SClasse c, SProfessore p) {
            return this.Weeks.Sum(w => w.Ore.Count(o => o.CheckCP(c, p)));
        }

        public int GetOre(SProfessore p) {
            return this.Weeks.Sum(w => w.Ore.Count(o => o.CheckCP(p)));
        }

        public int GetOre(SClasse c) {
            return this.Weeks.Sum(w => w.Ore.Count(o => o.CheckCP(c)));
        }

        private void CalcClassi() {
            var classi = new List<SClasse>();
            this._professori.ToList().ForEach(x => classi.AddRange(x.Classi));

            this.Classi = classi
                .GroupBy(x => $"{x}")
                .Select(x => new SClasse(x.Key, x.Sum(g => g.OreFrontali)))
                .ToArray();
        }

        public string[] Check(SProfessore p) {
            var tmp = new List<string>();
            foreach (var c in p.Classi) {
                var diff = this.GetDiffOre(c, p);
                if (diff != 0)
                    tmp.Add($"{c}[{c.OreFrontali}|{diff}]");
            }
            return tmp.ToArray();
        }

        public string[] Check(SClasse c) {
            var tmp = new List<string>();
            foreach (var p in _professori) {
                if (!p.CheckClasse(c)) continue;
                var tmpc = p.GetClasse(c);
                var diff = this.GetDiffOre(tmpc, p);
                if (diff != 0)
                    tmp.Add($"{p}[{tmpc.OreFrontali}|{diff}]");
            }
            return tmp.ToArray();
        }

        #endregion



        #region Tabelle

        

        public DataTable GetDataTableProfessori() {
            var table = new DataTable();
            table.Columns.Add(new DataColumn("PROFESSORE", typeof(string)));
            table.Columns.Add(new DataColumn("SIGLA", typeof(string)));
            table.Columns.Add(new DataColumn("CHECK", typeof(string)));

            var days = Enumerable.Range(0, NDay).ToArray();
            foreach (SWeek w in Weeks)
                foreach (int n in days)
                    table.Columns.Add(new DataColumn($"{w}{n + 1}", typeof(string)));
            
            _professori.ToList().ForEach(p => {
                var row = table.NewRow();

                var check = Check(p).ToList();
                if (check.Count() > 3) {
                    check = check.Take(3).ToList();
                    check.Add("..");
                }
                if (check.Count() == 0) check = new List<string>() { "OK" };

                row["PROFESSORE"] = p.Nome;
                row["SIGLA"] = $"{p}";
                row["CHECK"] = string.Join(" ", check);

                foreach (SWeek w in Weeks)
                    foreach (int n in days)
                        row[$"{w}{n + 1}"] = w.Found(p, n);

                table.Rows.Add(row);
            });
            return table;
        }


        public DataTable GetDataTableClasse() {
            var table = new DataTable();
            table.Columns.Add(new DataColumn("CLASSE", typeof(string)));
            table.Columns.Add(new DataColumn("CHECK", typeof(string)));

            var days = Enumerable.Range(0, NDay).ToArray();
            foreach (SWeek w in Weeks)
                foreach (int n in days)
                    table.Columns.Add(new DataColumn($"{w}{n + 1}", typeof(string)));


            Classi.ToList().ForEach(c => {
                var row = table.NewRow();

                var check = Check(c).ToList();
                if (check.Count() > 3) {
                    check = check.Take(3).ToList();
                    check.Add("..");
                }
                if (check.Count() == 0) check = new List<string>() { "OK" };

                row["CLASSE"] = $"{c}";
                row["CHECK"] = string.Join(" ", check);

                foreach (SWeek w in Weeks)
                    foreach (int n in days)
                        row[$"{w}{n + 1}"] = w.Found(c, n);

                table.Rows.Add(row);
            });
            return table;
        }
        

        public void DebugTableClasse() {
            var table = new ConsoleTable();
            table.Options.EnableCount = false;            
            table.Options.NumberAlignment = Alignment.Right;

            var columns = new[] { "CLASSE", "CHECK" }.ToList();
            
            var days = Enumerable.Range(0, NDay).ToArray();
            foreach (SWeek w in Weeks)
                foreach (int n in days)
                    columns.Add($"{w}{n + 1}");

            table.AddColumn(columns);

            Classi.ToList().ForEach(c => {
                
                var check = Check(c).ToList();
                if (check.Count() > 3) {
                    check = check.Take(3).ToList();
                    check.Add("..");
                }
                if (check.Count() == 0) check = new List<string>() { "OK" };

                var row = new[] {
                    $"{c}",
                    string.Join(" ", check),
                }.ToList();

                foreach (SWeek w in Weeks)
                    foreach (int n in days)
                        row.Add(w.Found(c, n));

                table.AddRow(row.ToArray());
            });

            table.Write(Format.Alternative);            
        }

        public void DebugTableProfessori() {
            var table = new ConsoleTable();
            table.Options.EnableCount = false;
            table.Options.NumberAlignment = Alignment.Right;

            var columns = new[] { "PROFESSORE", "CHECK" }.ToList();

            var days = Enumerable.Range(0, NDay).ToArray();
            foreach (SWeek w in Weeks)
                foreach (int n in days)
                    columns.Add($"{w}{n + 1}");

            table.AddColumn(columns);

            _professori.ToList().ForEach(p => {
                var check = Check(p).ToList();
                if (check.Count() > 3) {
                    check = check.Take(3).ToList();
                    check.Add("..");
                }
                if (check.Count() == 0) check = new List<string>() { "OK" };

                var row = new[] {
                    $"[{p}] {p.Nome}",
                    string.Join(" ", check),
                }.ToList();

                foreach (SWeek w in Weeks)
                    foreach (int n in days)
                        row.Add(w.Found(p, n));

                table.AddRow(row.ToArray());
            });

            table.Write(Format.Alternative);            
        }
        #endregion
    }
}
