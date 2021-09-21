namespace cqTimeSchool2 {
    public class SClasse {

        public SClasse(string classe, int oreFrontali) {
            _classe = classe;
            OreFrontali = oreFrontali;
        }

        private string _classe { get; set; }
        public int OreFrontali { get; set; }

        public override string ToString() {
            return _classe;
        }
    }

}