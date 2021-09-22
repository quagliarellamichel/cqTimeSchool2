# CODEQ TimeSchool V2
è un algoritmo in grado di calcolare l'orario scolastico automaticamente in pochi istanti 
e in pochi passaggi di configurazione
di seguito il download

### DOWNLOAD
troverai il file ZIP nella sezione release
oppure al seguente [LINK](https://github.com/quagliarellamichel/cqTimeSchool2/releases/download/v1.0/rel_v1.zip)

# CONFIGURAZIONE
tutti i file xml come "classe_1.xml" sono dei file di configurazione per impostare l'algoritmo 
definendo professori e classi
basterà aggiungere un nuovo file xml ad esempio classe_2.xml 
per permettere all'algoritmo di individuare il nuovo file e svolgere un nuovo calcolo

# TAG XML
```XML
<XML DayInWeek='6' SetupWeeks='LU,MA,ME,GI,VE' >
```
2 proprietà
- [DayInWeek] è possibile configurare il numero di ore lavorative per classe presenti in una settimana
- [SetupWeeks] è possibile selezionare i giorni della settimana LU,MA,ME,GI,VE,SA,DO (se si vogliono usare solo alcuni basterà rimuoverli dalla sequenza)

# TAG Professore
possono essere presenti più TAG Professore all'interno della root del file XML
```XML
<Professore Name='OCCHIPINTI G.' Sigla='OG'>
  <Classe Name='1B' OreFrontali='20' />
</Professore>
```
2 proprietà
- [Name] del professore
- [Sigla] (questa per essere visualizzata comodamente nella tabella dell'orario)
  
# TAG Classe
possono essere presenti più TAG Classe all'interno del professore
```XML
<Classe Name='1A' OreFrontali='8' />
```
2 proprietà
- [Name] sigla della classe esempio [1A]
- [OreFrontali] queste sono le ore frontali di insegnamento con gli alunni escluse le ore di compresenza che non verranno calcolate

# ESECUZIONE
la prima operazione richiesta sarà la selezione di uno dei file di configurazione trovati all'interno della cartella dell'eseguibile
![alt text](https://github.com/quagliarellamichel/cqTimeSchool2/blob/master/screen/s1.png?raw=true)
una volta completato il calcolo verrà visualizzato il risultato del calcolo a video
![alt text](https://github.com/quagliarellamichel/cqTimeSchool2/blob/master/screen/s2.png?raw=true)

# RISULTATO
verrà anche salvato un file excel con lo stesso nome del file configurazione con due pagine
### CLASSI
![alt text](https://github.com/quagliarellamichel/cqTimeSchool2/blob/master/screen/s3.png?raw=true)
### PROFESSORI
![alt text](https://github.com/quagliarellamichel/cqTimeSchool2/blob/master/screen/s4.png?raw=true)


# RINGRAZIAMENTO
ringrazio anticipatamente chi volesse fare una donazione 
per le ore di lavoro che l'algoritmo vi farà risparmiare
in basso il link per la donazione [PAYPAL](https://www.paypal.com/donate?hosted_button_id=4MAY2A7TYRCHW)

# POWERADE BY
ideato e sviluppato da  
  Quaglirella Michel
  Maggio 1991

per ulteriori info contattatemi via email: quagliarellamichel@gmail.com
