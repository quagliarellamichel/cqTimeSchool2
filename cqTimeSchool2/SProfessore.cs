using System;
using System.Linq;

namespace cqTimeSchool2 {
    public class SProfessore {
        public SProfessore(string nome, string sigla) {
            Nome = nome;
            _sigla = sigla;
        }

        public string Nome { get; }
        private string _sigla { get; }
        
        public SClasse[] Classi { get; set; } = new SClasse[0];

        public bool CheckClasse(SClasse c) {
            return this.Classi.Select(x => $"{x}").Contains($"{c}");
        }

        public SClasse GetClasse(SClasse c) {
            return this.Classi.FirstOrDefault(x => $"{x}" == $"{c}");
        }

        public override string ToString() {
            return _sigla;
        }
    }

}