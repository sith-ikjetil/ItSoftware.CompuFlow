# ItSoftware.CompuFlow.2017
CompuFlow is a producer/production-framework for all sorts of things like data generation, report production or simply 
a process of steps in any kind of information gathering/production process. The framework uses MSMQ to initiate 
and send the report to the next stage of production. The stages are Retrival, Generator, Publisher. At every stage 
there are also an Events stage that one can plug into if needed.

The framework works by the user creating a plugin-zip file that he drops into a special folder. Every stage has these 
plugins, and as soon as you drop your zip-file into the folder the framework pickes it up, extracts it and loads the 
plugin. It is then live. There are plugins for Retrival, Generator, Publisher and Events.

There is also an ASP.NET gateway which allows you to initiate a production process with. Either that or send an MSMQ 
message to the correct stages MSMQ queue. Every stage has their own queue.

The control center application allows you to see the state of the framework. What is in production and what each stage is 
currently working on. The control center must run on the machine where the framework is installed. This framework suits 
itself best installed on a server although it ceirtanly can be installed on a Windows workstation machine.
