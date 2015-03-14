# Merc Framework, Idioms & Best Practices #
(italian version)

## Ispirazione ##
Il Merc Framework nasce come tool per velocizzare sviluppo e manutenibilità di sistemi complessi e non. Le entità principali, i Merc, sono attive e comunicano tra loro scambiandosi messaggi. Ognuna di queste è un'automa a stati finiti che svolge un determinato compito all'interno dell'applicazione. Questo framework, nasce con con l'obiettivo di essere snello e povero di paradigmi, ma semplici e replicabili nelle situazioni più diverse. E' sviluppato in due linguaggi, Java e C# e di quest'ultimo ne è stata creata una versione aggiuntiva per il Compact.NET dei sistemi mobile.

## Paradigmi e punti di forza ##
Le seguenti caratteristiche sono in continuo divenire e vengono perfezionate nel susseguirsi dei progetti dove il framework viene utilizzato.

### Messaggi ###
Questo è il principale idioma. Rappresenta anche uno dei più utilizzati nelle architetture moderne, dove la chiamata a procedura non è più sufficiente soprattutto per i linguaggi su cui si sviluppano applicazioni necessariamente distribuite. Grazie al multithreading infatti, offerto dai più evoluti linguaggi ad oggetti utilizzati nell'implementazione del framework, lo scambio di messaggi assume un reale vantaggio di sviluppo, dove ogni entatità emittente o ricevente ottiene un lasco accoppiamento con le sue entità collaboratrici, bypassando i noti problemi di crashing dell'intero sistema in casi di anomalie per i singoli attori del sistema stesso. Inoltre avvicina drasticamente la tipologia di comunicazione locale con quella distribuita anche questa a messaggi per la natura del protocollo TCP/IP stesso (vedi sezione Sistema Distribuito). I messaggi possono essere inviati sia in modalità point to point che in modalità broadcast, dove ogni merc interessato alla tipologia inviata si troverà all'interno della propria coda il messaggio inviato.
I messaggi sono definiti attraverso tuple prolog, e l'unificazione fra queste e gli attributi (.net) o annotazioni (java), associate a metodi della classe del merc, permettono l'esecuzione (in ordine di arrivo) del messaggio.

### Entità autonome ###
I merc vengono registrati, all'avvio o durante l'esecuzione dell'applicazione, e 'prendono vita' ovvero ottengono un flusso di esecuzione alla ricezione del primo messaggio in modo da minimizzare le risorse acquisite all'avvio dal sistema. Una volta ottenuto il proprio flusso di esecuzione questo non farà altro che servire i messaggi in arrivo se associati in un determinato stato ad un determinato metodo. Inoltre ogni merc dispone di primitive per ispezionare la propria coda di messaggi in arrivo con filtri in base al messaggio e per eseguire il primo ricevuto sempre con la possibilità di specificarne la tipologia. Il merc ha sia la possibilità di 'killarsi' rilasciando il proprio thread ma anche di essere riavviato dal framework al prossimo messaggio oppure di deregistrarsi definitivamente dal sistema.

### Viste ###
Una delle principali motivazioni per la nascita del framework fu proprio quella di rendere le viste indipendenti dal resto della business logic. Si ricorda infatti che, all'avvio di applicazioni Form .NET, il flusso principale confluisce nella gestione delle finestre, dando così allo sviluppatore la possibilità di agire solo in seguito ad eventi scatenati dalle interfacce grafiche. Attraverso questo framework si ottiene la capacità di avere merc dedicati alla GUI indipendenti dal resto dei merc che si occuperanno di altre mansioni.
Questi 'FormMerc' contengono sempre metodi di risposta agli eventi della form ma inoltre attraverso un timer serviranno constantemente i messaggi in arrivo. In questo modo si evita anche il noioso utilizzo dei delegati dovuto dall'impossibilità di far eseguire operazioni sulla vista da parte di thread diversi dal creatore. Infatti il flusso che crea la vista sarà lo stesso che eseguirà i metodi di risposta ai messaggi.
In Java non è stato ancora pensato alcun paradigma per la gestione delle GUI lasciando allo sviluppatore la libertà di implementare la propria soluzione.

### ASF ###
Come anticipato, ogni merc è inoltre una macchina a stati. All'avvio è il framework che definisce lo stato di default con il quale avviarla. Il merc può cambiare stato e al suo cambiamento può essere associato un metodo (sempre attraverso attributi/annotazioni) da eseguire al cambiamento. E' buona pratica dello sviluppatore compiere all'interno del metodo di cambiamento stato solamente azioni riguardanti lo stato stesso. Lo stato influisce anche nella ricezioni di messaggi. Infatti se un messaggio è contemplato solamente per ben determinati stati, esso non verrà recapitato se il merc si trova in uno stato differente.

### Sistema Distribuito ###
Questa feature, non ancora completamente realizzata all'interno della versione java, è intuibile essere una estensione dovuta alla naturalezza con cui messaggi fra componenti di una applicazione locale possano iniziare a circolare anche fra componenti di applicazioni distribuite. L'idea è stata quella di continuare a vedere il Framework come base di appoggio unica sulla virtual machine locale del linguaggio ma con la possibilità di far circolare messaggi da e per i suoi merc verso merc di altri framework, presenti su differenti nodi della rete.

### Riusabilità ###
Un vantaggio indiscutibile dato dalla piattaforma descritta è quella di ampliare la riutilizzabilità concessa dalla programmazione ad oggetti. Infatti questa non verterà più sul singolo oggetto ma sull'intero Merc che potrà essere creato appositamente come interfaccia per device limitati, algoritmi complessi o utility applicative dove viene racchiusa la business logic all'interno di un automa (il merc) con un'interfaccia universale verso l'esterno (i messaggi).

## Sviluppo ##
L'intera piattaforma è composta da due progetti, ognuna di una decina di classi, la prima contenente utility e algoritmi per la gestione di messaggi come tuple prolog. La seconda invece mantiene tutto quello descritto sino ad'ora ed è aperta per l'utilizzo dei messaggi anche in un formato doverso dalla tupla.

### Tuple Engine (Prolog) ###
All'interno di questo progetto risultano i componenti principali del linguaggio prolog, Lexer & Parser, al fine di ottenere uno strumento per il matching e l'unificazione di tuple. Il matching infatti permette di identificare quale metodo del merc eseguire per un predeterminato messaggio arrivato da consegnare. L'unificazione invece fra messaggio e attributo/annotazione del metodo consente di assegnare agli argomenti del metodo gli argomenti della tupla.

### Merc Framework ###
Questo progetto contiene tutti le componenti del framework. Quelli principali sono descritti in seguito.
#### Classe Framework ####
Il compito di tale componente è quello di gestire la registrazione di ogni Merc e l'interesse da parte ognuno di essi dei messaggi che transiteranno per esso. Sarà suo compito anche spedire e distribuire i messaggi a cui i vari merc sono interessati. Prendendo in pasto un file di configurazione si occuperà di registrare merc all'avvio e spedire ad essi il messaggio iniziale se presente.
#### Classe Dispatcher ####
Forse la classe più importante di tutto il sistema, il dispatcher rappresenta l'interfaccia personale di ogni merc per il mondo esterno. Si occupa perciò di consegnare messaggi in partenza dal proprio merc al framework ma soprattuto di dare un flusso di esecuzione al merc all'arrivo del primo messaggio. In essa sono contenuti la coda dei messaggi (personale del merc) e la mappa dei metodi da eseguire al cambiamento di stato del merc.
#### Classe Merc ####
Da estendere ad ogni creazione di merc, fornisce il riferimento del dispatcher come interfaccia verso gli altri merc. Con l'implementazione dell'idioma ASF questa classe contiene anche lo stato del merc. Solo per C# esiste anche la classe FormMerc, da estendere per la realizzazione di merc che si occuperanno dell'interfaccia grafica. In quest'ultima è implementato il meccanismo per servire attraverso un timer (componente grafico .net) i messaggi in arrivo.

### Esempi C# ###
Il minimo per eseguire un'applicazione .NET che utilizza il framework risulta essere un main che inizializzi la classe Framework dopo aver linkato al progetto la libreria NetFramework.dll.

Attraverso il file di configurazione mercConfig.xml sarà possibile definire quali Merc registrare all'avvio e attraverso il file logging.config (da log4net) avere in console (con progetti Console Application) il logging di tutte le operazioni del framework all'interno dell'applicazione. Si riportano di seguito degli esempi applicativi.

#### Generica Classe Main ####
```
class Program
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace it.mintlab.desktopnet.mercframework.test
{
    class Program
    {
        static void Main(string[] args)
        {
            Framework f = new Framework();
        }
    }
}
```

#### Generico mercConfig.xml ####
```
<?xml version="1.0" encoding="utf-8" ?>
<config>
  <mercs>
    <merc name="Test" class="it.mintlab.desktopnet.mercframework.test.TestMerc, TestMultipleStateFilter" startmsg="init"/>
  </mercs>
</config>
```

#### Merc di test per il cambiamento di Stato ####
TestMerc.cs:

```
using System;
using System.Collections.Generic;
using System.Text;
using it.mintlab.desktopnet.mercframework;

namespace it.mintlab.desktopnet.mercframework.test
{
    class TestMerc : Merc
    {
        const string STATE_A = "STATE_A";
        const string STATE_B = "STATE_B";
        const string STATE_C = "STATE_C";


        [MessageBinding(message = "init")]
        public void init()
        {
            State = STATE_A;
        }

        [StateBinding(state = STATE_A)]
        public void stateA()
        {
            Console.WriteLine("State A");
            State = STATE_B;
        }

        [StateBinding(state = STATE_B)]
        public void stateB()
        {
            Console.WriteLine("State B");
            State = STATE_B;
        }

    }
}
```

#### Test Ricezione Messaggi basata sullo stato ####
TestMerc.cs:

```
using System;
using System.Collections.Generic;
using System.Text;
using it.mintlab.desktopnet.mercframework;

namespace it.mintlab.desktopnet.mercframework.test
{
    class TestMerc : Merc
    {
        const string STATE_A = "STATE_A";
        const string STATE_B = "STATE_B";
        const string STATE_C = "STATE_C";


        [MessageBinding(message = "init")]
        public void init()
        {
            State = STATE_A;
        }

        [StateBinding(state = STATE_A)]
        public void stateA()
        {
            Console.WriteLine("State A");
            State = STATE_B;
        }

        [StateBinding(state = STATE_B)]
        public void stateB()
        {
            Console.WriteLine("State B");
            State = STATE_B;
        }

    }
}

```
#### Test Merc Remoti ####
In questo esempio si immagini di avere un Merc "receiver" su un framework e un Merc "sender" su un altro framework remoto.

mercConfig.xml sul framework del receiver:

```
<?xml version="1.0" encoding="utf-8" ?>
<config>
  <framework-node port="6666"/>
  <mercs>
    <merc name="Receiver" class="it.mintlab.desktopnet.mercframework.test.remoting.ReceiverMerc, TestRemoteReceiver" startmsg="init"/>
    <merc name="ReceiverFriend" class="it.mintlab.desktopnet.mercframework.test.remoting.ReceiverFriendMerc, TestRemoteReceiver" startmsg="init"/>
    <remote-merc name="Sender" uri="http://localhost:6667/"></remote-merc>
  </mercs>
</config>
```

ReceiverMerc.cs:

```
using System;
using System.Collections.Generic;
using System.Text;
using it.mintlab.desktopnet.mercframework;

namespace it.mintlab.desktopnet.mercframework.test.remoting
{
    class ReceiverMerc : Merc
    {
        [MessageBinding(message = "init")]
        public void init()
        {
            dispatcher.logInfo("Waiting for friend..");
        }

        [MessageBinding(message = "hello")]
        public void hello()
        {
            dispatcher.deliverMessage("Sender", "question('How are you?')");
        }

        [MessageBinding(message = "response(_msg)")]
        public void response(string msg)
        {
        }

        [MessageBinding(message = "question(_msg)")]
        public void question(string msg)
        {
            dispatcher.deliverMessage("Sender", "response('I do not know')");
        }

        [MessageBinding(message = "bye")]
        public void bye()
        {
            dispatcher.killMerc();
        }
    }
}
```

mercConfig.xml sul framework del sender:

```
<?xml version="1.0" encoding="utf-8" ?>
<config>
  <framework-node port="6667"/>
  <mercs>
    <merc name="Sender" class="it.mintlab.desktopnet.mercframework.test.remoting.SenderMerc, TestRemoteSender" startmsg="init"/>
    <remote-merc name="Receiver" uri="http://localhost:6666/"></remote-merc>
  </mercs>
</config>
```

SenderMerc.cs:

```
sing System;
using System.Collections.Generic;
using System.Text;
using it.mintlab.desktopnet.mercframework;

namespace it.mintlab.desktopnet.mercframework.test.remoting
{
    class SenderMerc : Merc
    {
        [MessageBinding(message = "init")]
        public void init() 
        {
            dispatcher.deliverMessage("Receiver", "hello");
        }

        [MessageBinding(message = "hello")]
        public void hello()
        {
        }

        [MessageBinding(message = "question(_msg)")]
        public void question(string msg)
        {
            dispatcher.deliverMessage("Receiver", "response('Not bad..')");
            dispatcher.deliverMessage(Message.BROADCAST_REMOTE, "question('What are you doing, guys?')");
        }

        [MessageBinding(message = "response(_msg)")]
        public void response(string msg)
        {
            dispatcher.deliverMessage(Message.BROADCAST_ALL, "bye");
            dispatcher.killMerc();
        }

    }
}
```
#### Test Registrazione di merc a Runtime ####
mercConfig.xml:

```
<?xml version="1.0" encoding="utf-8" ?>
<config>
  <mercs>
    <merc name="Registrant" class="it.mintlab.desktopnet.mercframework.test.RegistrantMerc, TestRuntimeRegistration" startmsg="init"/>
  </mercs>
</config>
```

RegistrantMerc.cs:

```
using System;
using System.Collections.Generic;
using System.Text;
using it.mintlab.desktopnet.mercframework;

namespace it.mintlab.desktopnet.mercframework.test
{
    public class RegistrantMerc : Merc
    {
        [MessageBinding(message = "init")]
        public void init()
        {
            dispatcher.deliverMessage("Registered", "anybody");
            dispatcher.registerMerc("Registered", "it.mintlab.desktopnet.mercframework.test.RegisteredMerc, TestRuntimeRegistration");
            dispatcher.deliverMessage("Registered", "andNow");
            dispatcher.deliverMessage("Registered", "deregisterYou");
            dispatcher.killMerc();
        }
    }
}
```

RegisteredMerc.cs:

```
using System;
using System.Collections.Generic;
using System.Text;
using it.mintlab.desktopnet.mercframework;

namespace it.mintlab.desktopnet.mercframework.test
{
    public class RegisteredMerc : Merc
    {
        [MessageBinding(message = "anybody")]
        public void m1()
        {
            dispatcher.logInfo("Test Error");
            dispatcher.registerMerc("Registered", "it.mintlab.desktopnet.mercframework.test.RegistrantMerc, TestRuntimeRegistration");
        }

        [MessageBinding(message = "andNow")]
        public void m2()
        {
            dispatcher.logInfo("Test OK");
        }

        [MessageBinding(message = "deregisterYou")]
        public void m3()
        {
            dispatcher.deregisterMerc();
        }

    }
}
```