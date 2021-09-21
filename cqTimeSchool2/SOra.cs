using System;
using System.Collections.Generic;
using System.Linq;

namespace cqTimeSchool2 {
    public class SOra {
        
        public SOra(int index) {
            Index = index;
        }

        public int Index { get; set; }        
        private List<CP> _cp { get; set; } = new List<CP>();

        public bool AddCP(SClasse c, SProfessore p, bool error = false) {
            if (CheckCP(c)) return false;
            if (!error) {                
                if (CheckCP(p)) return false;
            }            
            _cp.Add(new CP(c, p));
            return true;
        }

        public override string ToString() {
            return $"{Index}";
        }

        public bool CheckCP(SClasse c, SProfessore p) {
            return GetCPsGood().Count(x => $"{x.Classe}" == $"{c}" && $"{x.Prof}" == $"{p}") > 0;
        }

        public bool CheckCPErr(SClasse c, SProfessore p) {
            return _cp.Count(x => $"{x.Classe}" == $"{c}" && $"{x.Prof}" == $"{p}") > 0;
        }

        public bool CheckCP(SClasse c) {
            return _cp.Count(x => $"{x.Classe}" == $"{c}") > 0;
        }
        
        public bool CheckCP(SProfessore p) {
            return _cp.Count(x => $"{x.Prof}" == $"{p}") > 0;
        }

        /*
        public CP GetCP(PClasse c) {
            return _cp.FirstOrDefault(x => x.Classe == c.Classe);
        }

        public CP GetCP(SProfessore p) {
            return _cp.FirstOrDefault(x => x.Prof == $"{p}");
        }
        */

        public string GetStr(SClasse c) {
            var tmp = _cp.Where(x => $"{x.Classe}" == $"{c}").Select(x => $"{x.Prof}").ToList();            
            if (tmp.Count() > 3) {
                tmp = tmp.Take(3).ToList();
                tmp.Add("..");
            }
            return string.Join("|", tmp);
        }

        public string GetStr(SProfessore p) {
            var tmp = _cp.Where(x => $"{x.Prof}" == $"{p}").Select(x => $"{x.Classe}").ToList();
            if (tmp.Count() > 3) {
                tmp = tmp.Take(3).ToList();
                tmp.Add("..");
            }
            return string.Join("|", tmp);
        }

        public CP[] GetCPsGood() {
            return _cp
                .GroupBy(x => $"{x.Prof}")
                .Where(x => x.Count() == 1)
                .Select(x => x.First())
                .ToArray();
        }

        /*
        public CP[] GetCPsError() {
            return _cp.ToArray();
        }*/

        public void RemoveCP(SClasse c, SProfessore p) {
            var cp = _cp.FirstOrDefault(x => $"{x.Prof}" == $"{p}" && $"{x.Classe}" == $"{c}");
            if (cp == null) return;
            _cp.Remove(cp);
        }

        /// <summary>
        /// conta solo i cp che non hanno errori
        /// </summary>
        /// <returns></returns>
        public int CountCP() {            
            return GetCPsGood().Count();
        }

        /// <summary>
        /// trova tutti gli errori di sovrapposizione tra Professore Classi
        /// </summary>
        /// <returns></returns>
        public ErrPC[] GetCPError() {
            return _cp
                .GroupBy(x => $"{x.Prof}")
                .Where(x => x.Count() > 1)
                .Select(k => new ErrPC(this.Index, k.First().Prof, k.Select(x => x.Classe).ToArray()))
                .ToArray();
        }

        /*
        public CP[] FoundCPsGood(SClasse c, SProfessore p) {            
            var cpsGood = GetCPsGood();
            if (cpsGood.Count(x => $"{x.Prof}" == $"{p}") > 0) return new CP[0];
            return cpsGood
                .Where(x => $"{x.Classe}" == $"{c}")
                .ToArray();
        }
        */
    }
}