# Yort.DeadManSwitch

## What is it?
A reusable implementation of a dead man switch. A dead man switch 'activates' (causes something to happen) when some event hasn't occurred for a specified period of time. It is effectively a resetable timer.

### When would I use it?
Any time you want to take some action because something else hasn't happened for a while, this includes but is not limited to;

* Logging or sending an alert when you haven't received a (network or other) message for X minutes/hours.
* Performing a search x millseconds after the last key pressed in an auto-search/complete text field.
* Starting archive/optimisation processes when there's been no user activity for a while.

[![GitHub license](https://img.shields.io/github/license/mashape/apistatus.svg)](https://github.com/Yortw/Yort.DeadManSwitch/blob/master/LICENSE) 

[![Build status](https://ci.appveyor.com/api/projects/status/orbrd1dkd2s40dm1?svg=true)](https://ci.appveyor.com/project/Yortw/yort-deadmanswitch)

## Supported Platforms
Currently;

* .Net Standard 2.0 
* .Net 4.0+

## Available on Nuget

```powershell
    PM> Install-Package Yort.DeadManSwitch
```
[![NuGet Badge](https://buildstats.info/nuget/Yort.DeadManSwitch)](https://www.nuget.org/packages/Yort.DeadManSwitch/)

## How do I use it?

Create a switch passing the action to call when the switch activates, the delay before activation, and other settings to the constructor. Call the *Reset* method of the switch each time a regular event occurs. When *Reset* hasn't been called within the delay period specified, the switch will activate.

*See the demo console app in the repo for sample usage*

```c#
    using Yort.DeadManSwitch;

    //This switch activates after 5 seconds on inactivity, and automatically
    //resets itself after activation.
    var dms = new DeadManSwitch(5000, () => Console.WriteLine("Switch activated!")), (reason) => Console.WriteLine("Reset because " + reason.ToString()), true);

    //Somewhere else in the code, in a code path that should execute regularly within 5 seconds
    dms.Reset();

    //To stop the switch, dispose it.
    dms.Dispose();
```
