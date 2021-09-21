
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;

namespace Connect {
    public static class Conv {

        public static int ParseAnno(string txt, int def, int? max = 9999, int? min = 2000) {
            var anno = ParseInt(txt, def);
            anno = Math.Abs(anno);
            if ($"{anno}".Length == 2) anno += 2000;
            if (max != null && anno > max) anno = def;
            if (min != null && anno < min) anno = def;
            return anno;
        }


        public static int ParseInt(string val, int def = 0, int? min = null, int? max = null) {
            if (string.IsNullOrEmpty(val)) return 0;
            //val = FixSpecialChar(val);
            int x;
            try {
                x = int.Parse(FixVirgola(val, false), CultureInfo.InvariantCulture);
            } catch (Exception) {
                x = def;
            }
            if (x < min) x = def;
            if (x > max) x = def;
            return x;
        }

        public static int? ParseIntNull(string val, int? def = null, int? min = null, int? max = null) {
            int? x;
            if (string.IsNullOrEmpty(val)) return null;
            //val = FixSpecialChar(val);
            try {
                x = int.Parse(FixVirgola(val, false), CultureInfo.InvariantCulture);
            } catch (Exception) {
                x = def;
            }
            if (x < min) x = def;
            if (x > max) x = def;
            return x;
        }

        public static decimal ParseDecimal(string val, decimal def = 0, decimal? min = null, decimal? max = null) {
            //val = FixSpecialChar(val);
            decimal x;
            try {
                x = decimal.Parse(FixVirgola(val), CultureInfo.InvariantCulture);
            } catch (Exception) {
                x = def;
            }
            if (x < min) x = def;
            if (x > max) x = def;
            return x;
        }

        public static double ParseDouble(string val, double def = 0, double? min = null, double? max = null, int? round = null) {
            //val = FixSpecialChar(val);
            double x;
            try {
                x = double.Parse(FixVirgola(val), CultureInfo.InvariantCulture);
            } catch (Exception) {
                x = def;
            }
            if (x < min) x = def;
            if (x > max) x = def;
            if (round != null) x = Math.Round(x, round ?? 0);
            return x;
        }

        public static double? ParseDoubleNull(string val, double? def = null, double? min = null, double? max = null, int? round = null) {
            //val = FixSpecialChar(val);
            double? x;
            if (string.IsNullOrEmpty(val)) return null;
            try {
                x = double.Parse(FixVirgola(val), CultureInfo.InvariantCulture);
            } catch (Exception) {
                x = def;
            }
            if (x < min) x = def;
            if (x > max) x = def;
            if (round != null && x != null)
                x = Math.Round((double)x, round ?? 0);
            return x;
        }

        public static DateTime ParseDate(string val, DateTime def = new DateTime(), DateTime? min = null, DateTime? max = null) {
            if (ParseInt(val) != 0) val += $"/{DateTime.Today.Month}/{DateTime.Today.Year}";
            DateTime x;
            try {
                x = DateTime.Parse(val, CultureInfo.InvariantCulture);
            } catch (Exception) {
                x = def;
            }
            if (min != null && x < min) x = def;
            if (max != null && x > max) x = def;
            return x;
        }

        public static DateTime? ParseDateNull(string val, DateTime? def = null, DateTime? min = null, DateTime? max = null) {
            DateTime? x;
            if (string.IsNullOrEmpty(val)) return null;
            if (ParseInt(val) != 0) val += $"/{DateTime.Today.Month}/{DateTime.Today.Year}";
            try {
                x = DateTime.Parse(val, CultureInfo.InvariantCulture);
            } catch (Exception) {
                x = def;
            }
            if (min != null && x < min) x = def;
            if (max != null && x > max) x = def;
            return x;
        }

        public static float ParseFloat(string val, float def = 0, float? min = null, float? max = null) {
            //val = FixSpecialChar(val);
            float x;
            try {
                x = float.Parse(FixVirgola(val), CultureInfo.InvariantCulture);
            } catch (Exception ex) {
                Console.WriteLine(ex);
                x = def;
            }
            if (min < x) x = def;
            if (max > x) x = def;
            return x;
        }

        public static float? ParseFloatNull(string val, float def = 0, float? min = null, float? max = null) {
            //val = FixSpecialChar(val);
            float? x;
            if (string.IsNullOrEmpty(val)) return null;
            try {
                x = float.Parse(FixVirgola(val), CultureInfo.InvariantCulture);
            } catch (Exception) {
                x = def;
            }
            if (x < min) x = def;
            if (x > max) x = def;
            return x;
        }

        public static bool ParseBool(string val, bool def = false) {
            bool x;
            try {
                if (val == "1") val = "true";
                x = bool.Parse(val);
            } catch (Exception) {
                x = def;
            }
            return x;
        }


        //private static string FixSpecialChar(string val) => val?.Replace("'", "").Replace("%", "").Replace("€", "") ?? "";

        /// <summary>
        /// se i char con la virgola sono maggiori di uno cambia l'ultima virgola con il carattere § rimuovi tutte le virgole presenti e reinserisci la virgola al posto del carattere speciale
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        private static string FixVirgola(string val, bool aceptDot = true) {
            try {
                var charAcept = "0123456789".ToArray();

                val = val.Replace(',', '.');
                var tmp = val.ToCharArray().Select((x, pos) => new CPE(x, pos, false)).ToList();
                List<CPE> punti = tmp.Where(x => x.C == '.').ToList();

                foreach (var t in tmp)
                    if (charAcept.Contains(t.C))
                        tmp[t.P].E = true;

                if (aceptDot)
                    tmp[punti.Last().P].E = true;

                var arr = tmp.Where(x => x.E == true).Select(x => $"{x.C}").ToArray();
                return string.Join("", arr);
            } catch {
                return val;
            }
        }

        public class CPE {
            public CPE(char c, int p, bool e) {
                C = c;
                P = p;
                E = e;
            }
            public char C { get; set; }
            public int P { get; set; }
            public bool E { get; set; }
        }
    }
}
