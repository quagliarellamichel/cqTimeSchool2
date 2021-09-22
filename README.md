# CODEQ TimeSchool V2
è un algoritmo in grado di calcolare l'orario scolastico,
configurando semplicemente i professori e le classi a essi associati,
quest'ultimo eliminerà automaticamente le sovrapposizioni e le incongruenze.

### DOWNLOAD
troverai il file ZIP nella sezione release
[LINK](https://github.com/quagliarellamichel/cqTimeSchool2/releases/download/v1.0/rel_v1.zip)

# CONFIGURAZIONE
tutti i file xml come "classe_1.xml" determinano la configurazione dell'algoritmo, definendo professori e classi
basterà aggiungere un nuovo file xml ad esempio classe_2.xml 
per permettere all'algoritmo d'individuare il nuovo file e svolgere il calcolo

# TAG XML
```XML
<XML DayInWeek='6' SetupWeeks='LU,MA,ME,GI,VE' >
```
proprietà
- [DayInWeek] imposta il numero di ore lavorative in una settimana.
- [SetupWeeks] imposta i giorni della settimana (LU,MA,ME,GI,VE,SA,DO) per escludere basterà rimuoverli dalla sequenza.

# TAG Professore
possono essere presenti più TAG Professore all'interno della root del file XML
```XML
<Professore Name='OCCHIPINTI G.' Sigla='OG'>
  <Classe Name='1B' OreFrontali='20' />
</Professore>
```
proprietà
- [Name] Nome del professore
- [Sigla] Le iniziali del professore verranno visualizzati nella tabella dell'orario
  
# TAG Classe
possono essere presenti più TAG Classe all'interno del professore
```XML
<Classe Name='1A' OreFrontali='8' />
```
proprietà
- [Name] Nome della classe, esempio [1A]
- [OreFrontali] ore frontali dell'insegnante nella classe (le compresenze non verranno calcolate)

# ESECUZIONE
la prima operazione richiesta sarà la selezione di uno dei file di configurazione trovati all'interno della cartella dell'eseguibile
![alt text](https://github.com/quagliarellamichel/cqTimeSchool2/blob/master/screen/s1.png?raw=true)
una volta completato il calcolo verrà visualizzato il risultato a video
![alt text](https://github.com/quagliarellamichel/cqTimeSchool2/blob/master/screen/s2.png?raw=true)

# RISULTATO
verrà anche salvato un file excel con lo stesso nome della configurazione con due pagine
### CLASSI
![alt text](https://github.com/quagliarellamichel/cqTimeSchool2/blob/master/screen/s3.png?raw=true)
### PROFESSORI
![alt text](https://github.com/quagliarellamichel/cqTimeSchool2/blob/master/screen/s4.png?raw=true)


# RINGRAZIAMENTO
ringrazio anticipatamente chi volesse fare una donazione 
per le ore di lavoro che l'algoritmo vi farà risparmiare
in basso il link per la donazione
[PAYPAL](https://www.paypal.com/donate?hosted_button_id=4MAY2A7TYRCHW)

# POWERADE BY
ideato sviluppato da Quaglirella Michel (Maggio 91)
per ulteriori info contattatemi via email: quagliarellamichel@gmail.com
