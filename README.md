DotNetOMIDemo
=============

DotNetOMIDemo is a simple demo of using the
[SAS® Open Metadata Interface](http://support.sas.com/documentation/cdl/en/omaref/63063/HTML/default/viewer.htm#titlepage.htm)
(OMI) from the .NET platform.

The SAS Open Metadata Interface allows programmers to interoperate with a SAS Metadata Server
to query, add, update and delete metadata for a SAS platform installation.

Whilst I've spent a few years working with the SAS OMI from Java, I've only recently started 
using it on the .NET platform. There are plenty of Java based examples of using the SAS OMI but I 
couldn't find any .NET examples. This demo serves as a .NET example for me to refer back to when 
I need it and perhaps might also help others too. 

It's a console application which connects to a metadata server, using connection attributes specified
on the command line, to execute an IOMI method and print the resulting XML to the console.

The demo uses the lower level IOMI class to work with the SAS metadata server through its XML 
interface. It shows how to obtain a connection to a SAS metadata server programmatically (without
using any XML config files) and then execute an IOMI method. Currently it only calls the IOMI
[GetRepositories](http://support.sas.com/documentation/cdl/en/omaref/63063/HTML/default/viewer.htm#n0b9vyxiwd9cgkn1dzm8018mrkx2.htm)
method as a very simple example to list the available metadata repositories.
I may update the demo in future with examples of additional IOMI method calls.

This git repository depends upon but does not include any SAS software or libraries. 
If you want to build and run this demo you must have seperately licensed and installed
SAS software from [SAS Insitute Inc](http://www.sas.com/). This demo is not associated with,
endorsed by, or sponsored by SAS Institute Inc. and has no official or unofficial affiliation
with SAS Institute Inc.

License
=======

DotNetOMIDemo is licensed under the terms of the [MIT License](http://opensource.org/licenses/MIT).
See LICENSE.md for more information.

DotNetOMIDemo includes source code from the
[Command Line Parser Library](https://github.com/gsscoder/commandline)
Copyright (c) 2005 - 2012 Giacomo Stelluti Scala.
Command Line Parser Library is also licensed under the MIT License.

Dependancies
============

This demo has the following dependancies:

### Microsoft Visual Studio and .NET Framework

You will need [Microsoft Visual Studio](http://www.microsoft.com/visualstudio) in order to build the demo.
I used the free _Microsoft Visual Studio Express 2012 for Windows Desktop_.
The demo has been configured to use the .NET Framework Version 4.

### Command Line Parser Library

DotNetOMIDemo uses the [Command Line Parser Library](https://github.com/gsscoder/commandline)
for it's command line option handling capabilities. For convenience this dependancy has been
handled via source code inclusion as documented [here](https://github.com/gsscoder/commandline/wiki/Quickstart).
No additional steps are requried to handle this dependancy, but if a newer version is available you may want to
fork this demo to update the commandline source code or convert it to an assembly dependancy instead.

### SAS Integration Technologies

In addition to having a SAS platform installation with a SAS metadata server to connect to, 
in order to build and run this demo, you will need to have installed the _SAS Integration
Technologies Client_. This client provides the necessary SAS assemblies that constitute the
.NET callable SAS Open Metadata Interface.
You will already have the _SAS Integration Technologies Client_ installed if you have installed 
_SAS Enterprise Guide_ or the _SAS Add-in for Microsoft Office_.


Building the Demo
=================

Open the _DotNetOMIDemo.sln_ file in Microsoft Visual Studio. Resolve any broken references
to the SAS assemblies by pointing to the location of your _SAS Integration Technologies Client_
installation. The demo includes references to the following SAS assemblies:
* SASOMIInterop.dll (which for me is in C:\Program Files\SAS\SharedFiles\Integration Technologies)

Use the _BUILD_ > _Build Solution_ menu items to generate the _DotNetOMIDemo.exe_ file in
the _bin/Release_ directory. If you have the SAS Integration Technologies Client references
correctly resolving you should also find some SAS DLLs have been copied into this directory
too.

Running the Demo
================

Once you have successfully built the demo you can run the _DotNetOMIDemo.exe_ command from
the _bin/Release_ directory. You will need to provide the appropriate connection attributes
for your SAS metadata server. This includes the metadata server's host name and port number
as well as an appropriate user id and password. The password can be specified as plain
text or in SAS
[pwencode](http://platformadmin.com/blogs/paul/2012/06/password-encoding-with-sas/) format.

### Usage

Usage information, together with command line options, can be obtained by running
`DotNetOMIDemo.exe --help` which will generate the following output:

    DotNetOMIDemo 1.0
    Copyright © 2012 Paul Homes
    DotNetOMIDemo is licensed under the terms of the MIT License
    <http://opensource.org/licenses/MIT>.
    Usage: DotNetOMIDemo.exe --host=<hostname> --port=<port> --user=<user> --password=<password>
           DotNetOMIDemo.exe --host localhost --port=8561 --user='sasadm@saspw' --password='secret'

      -h, --host        SAS metadata server host name (default=localhost)
    
      -t, --port        SAS metadata server port number (default=8561)
    
      -u, --user        Required. SAS metadata server user id
    
      -p, --password    Required. SAS metadata server password
    
      -v, --verbose     Enable verbose output
    
      --help            Display this help screen.
  
### Examples

Here are a couple of command line examples:

    bin\Release\DotNetOMIDemo.exe -h sasmeta.example.com -t 8563 -u sasdemo -p secret
    
    bin\Release\DotNetOMIDemo.exe --host="sasmeta.example.com" --port=8563 --user="sasdemo" --password="{SAS002}B87C6F3C16DD10DE179AC3C3"

Reference Documentation
=======================

You may find the following documentation references useful when reviewing or extending this demo:

* _SAS® 9.3 Integration Technologies: Windows Client Developer's Guide_:
  * [Client Installation](http://support.sas.com/documentation/cdl/en/itechwcdg/62763/HTML/default/viewer.htm#p1bixi4v7enxw0n1hctlxhlbjzcd.htm)
  * [Programming in the .NET Environment](http://support.sas.com/documentation/cdl/en/itechwcdg/62763/HTML/default/viewer.htm#p1t48e0y92v4jqn1csygma6z7178.htm)
* _SAS® 9.3 Open Metadata Interface: Reference and Usage_: [Metadata Access (IOMI Interface)](http://support.sas.com/documentation/cdl/en/omaref/63063/HTML/default/viewer.htm#p1i3uxspauhglsn1osxyp1bu9j7q.htm)
* [SAS® 9.3 Metadata Model: Reference](http://support.sas.com/documentation/cdl/en/omamodref/63903/HTML/default/viewer.htm#titlepage.htm)
* SASObjectManager Help File from the locally installed _SAS Integration Technologies Client_ (e.g. C:\Program Files\SAS\SharedFiles\Integration Technologies\sasoman.chm)


Trademarks
==========

SAS and all other SAS Institute Inc. product or service names are registered
trademarks or trademarks of [SAS Institute Inc.](http://www.sas.com/) in the
USA and other countries. ® indicates USA registration.

Other product and company names mentioned herein may be registered trademarks
or trademarks of their respective owners.
