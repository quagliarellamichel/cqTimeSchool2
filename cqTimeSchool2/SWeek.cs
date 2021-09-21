using System;
using System.Collections.Generic;
using System.Linq;

namespace cqTimeSchool2 {
    public class SWeek {

        public SWeek(WeekDays nome, int nOre) {
            Nome = nome;            
            Ore = Enumerable.Range(0, nOre).Select(index => new SOra(index)).ToArray();
        }

        public WeekDays Nome { get; set; }
        public SOra[] Ore { get; set; }

        public static SWeek[] GenerateWeekEmpty(WeekDays[] configWeeks, int nDay) {
            var tmp = new List<SWeek>();
            configWeeks.ToList().ForEach(x => {
                tmp.Add(new SWeek(x, nDay));
            });
            return tmp.ToArray();
        }
        
        public string Found(SProfessore p, int index) {
            return Ore[index].GetStr(p);
        }

        public string Found(SClasse c, int index) {
            return Ore[index].GetStr(c);
        }

        public SolutionP[] FoundSolutionCP(int eIndex, SProfessore eProf, SClasse classe) {
            /* posizione iniziale             
             * +----------------------------------+ 
             * |            | WEEK1               |
             * +------------+------+--------------+ 
             * |PROF        |ORA_1 |ORA_2 (eIndex)|              
             * +------------+------+--------------+
             * |P1          |1B    |              |
             * +------------+------+--------------+
             * |P2 (eProf)  |      |1B-1C         | 
             * +------------+------+--------------+
             */

            /* soluzione finale
             * +----------------------------------+ 
             * |            | WEEK1 1B(classe)    |
             * +------------+------+--------------+ 
             * |PROF        |ORA_1 |ORA_2 (eIndex)|              
             * +------------+------+--------------+
             * |P1          |      |1B            |
             * +------------+------+--------------+
             * |P2 (eProf)  |1B    |1C            |
             * +------------+------+--------------+
             */

            /*
             * 1B è la classe che voglio risolvere dalla sovrapposizione
             * condizione di ricerca
             * P1 nella posizione ORA_2 deve essere libero
             * P2 nella posizione ORA_1 deve essere libero
             * devi prendere in esame i professori che sono liberi nell'ORA_2
             * devi prendere in esame i professori che al di fuori dell'ORA_2 hanno orari assegnati alla 1B
             */

            var eOra = Ore[eIndex];
            return Ore
                // togli dal calcolo l'ORA_2
                .Where(ora => ora.Index != eIndex)
                //nell'ORA_1 il P2(eProf) deve essere libero
                .Where(ora => !ora.CheckCP(eProf))
                .Select(ora => 
                    new SolutionP(
                        //index ORA_1
                        ora.Index,
                        //distanza tra ORA_1 e ORA_2(eIndex)
                        Math.Abs(eIndex - ora.Index),
                        //la sovrapposizione è data da 1B-1C ma io voglio risolvere 1B e lasciare 1C al suo posto
                        classe, 
                        // trova il professore P1 nell'ORA_1
                        ora
                            // trova tutti i cp(classe professore) correttamente assegnati senza errore
                            .GetCPsGood()
                            //prendi i CP che contengono la classe 1B ma non devono appartenere a P2(eProf)
                            .Where(cp => $"{cp.Classe}" == $"{classe}" && $"{cp.Prof}" != $"{eProf}")                            
                            //P1 nella posizione ORA_2(eIndex) deve essere libero
                            .Where(cp => !eOra.CheckCP(cp.Prof))
                            //seleziona P1
                            .Select(cp => cp.Prof)
                            .ToArray()
                ))
                //togli tutte le soluzioni calcolate che non hanno professori
                .Where(x => x.Profs.Count() > 0)
                .ToArray();
        }

        public ErrPC[] GetCPError() {
            var tmp = new List<ErrPC>();
            this.Ore.ToList().ForEach(x => tmp.AddRange(x.GetCPError()));
            return tmp.ToArray();
        }

        public override string ToString() {
            return $"{Nome}";
        }
    }



    public class CP {
        public CP(SClasse c, SProfessore p) {
            Classe = c;
            Prof = p;
        }
        public SClasse Classe { get; set; } = null;
        public SProfessore Prof { get; set; } = null;
    }

    public class SolutionP {
        public SolutionP(int index, int delta, SClasse c, params SProfessore[] ps) {
            Index = index;
            Delta = delta;
            Classe = c;
            Profs = ps;
        }

        public int Delta { get; set; }
        public int Index { get; set; }
        public SClasse Classe { get; set; }
        public SProfessore[] Profs { get; set; }
    }

    public class ErrPC {
        public ErrPC(int index, SProfessore prof, params SClasse[] classi) {
            Classi = classi;
            Index = index;
            Prof = prof;
        }

        public SClasse[] Classi { get; set; } = new SClasse[0];
        public SProfessore Prof { get; set; } = null;

        public int Index { get; set; } = -1;
    }
}