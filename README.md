Introduction
------------
OpenPasswordFilter is an open source custom password filter DLL and userspace service to better protect / control Active Directory domain passwords. 

[Troy Hunt](https://twitter.com/troyhunt) has wrote a [blog post](https://haveibeenpwned.com/Passwords) where he published 324+ millions of password hashes from breaches in past, so what I did is that I took those files, loaded them in SQL database and modified OPF to query those 
instead of password lists as in the [original project](https://github.com/jephthai/OpenPasswordFilter). 

I have added configuration options in OPFilterService.config so you can configure OPF to use either of those, enable logging, etc.

You can find the original project tree [here](https://github.com/jephthai/OpenPasswordFilter). 

Accompanied blog post about installation and more detailed instructions can be found on my [blog](https://amarkulo.com/integrating-database-of-pwned-password-hashes-with-microsoft-ad).

About OPF
------------
Here is the descriptive part of readme from the original project.

The genesis of this idea comes from conducting many penetration tests where organizations have users who choose common passwords
and the ultimate difficulty of controlling this behavior.  The fact is that any domain of size will have some user who chose
`Password1` or `Summer2015` or `Company123` as their password.  Any intruder or low-privilege user who can guess or obtain
usernames for the domain can easily run through these very common passwords and start expanding the level of access in the 
domain.

Microsoft provides a wonderful feature in Active Directory, which is the ability to create a custom password filter DLL.  This
DLL is loaded by LSASS on boot (if configured), and will be queried for each new password users attempt to set.  The DLL simply
replies with a `TRUE` or `FALSE`, as appropriate, to indicate that the password passes or fails the test.  

There are some commercial options, but they are usually in the "call for pricing" category, and that makes it a little 
prohibitive for some organizations to implement truly effective preventive controls for this class of very common bad passwords. 

This is where OpenPasswordFilter comes in -- an open source solution to add basic dictionary-based rejection of common
passwords.

OPF is comprised of two main parts:

   1. OpenPasswordFilter.dll -- this is a custom password filter DLL that can be loaded by LSASS to vet incoming password changes.
   2. OPFService.exe -- this is a C#-based service binary that provides a local user-space service for maintaining the dictionary and servicing requests.
  
The DLL communicates with the service on the loopback network interface to check passwords against the configured database
of forbidden values.  This architecture is selected because it is difficult to reload the DLL after boot, and administrators
are likely loathe to reboot their DCs when they want to add another forbidden password to the list.  Just bear in mind how this
architecture works so you understand what's going on.

**NOTE** The current version is very ALPHA!  I have tested it on some of my DCs, but your mileage may vary and you may wish to
test in a safe location before using this in real life.

Installation
------------
You can download a precompiled 32 and 64-bit version of OPF from the following links:

| File          | MD5 sum     | SHA1 sum  |
| ------------- |-------------| ----------|
| [x64.7z](https://github.com/amarkulo/OpenPasswordFilter/raw/master/x64.7z) | 69aeafa5a543f28a542345e621c1b8ab | 8af16eafa2d9b136b3d77ae4b380667f498b3d17 |
| [x86.7z](https://github.com/amarkulo/OpenPasswordFilter/raw/master/x86.7z)| 48dd6bc5980201e8e20aabfcf89d1d70 | 67f9e18ba974b6fdf60fd64252438c5e0e0cb8f5 |



Please verify hashes before putting them on your servers.
    
TL;DR:

  1. Copy complete release catalog to some place on disk
  2. Run `\windows\Microsoft.NET\Framework64\v4.0.30319\InstallUtil.exe OPFService.exe` command to install the service
  3. Start the service
  4. Copy `OpenPasswordFilter.dll` to `%WINDIR%\System32`
  5. Validate that `HKLM\SYSTEM\CurrentControlSet\Control\Lsa\Notification Packages` registry key contains OpenPasswordFilter value
  6. Configure SQL settings in app.config
  7. Start the service and test if it works with provided OPFTest.exe which queries service using OpenPasswordFilter.dll
  8. Restart the DC server so changes to LSA takes place
  9. Repeat for the rest of your DC servers
  
If all has gone well, test by using the normal GUI password reset function (ctrl+alt+del) to choose a password that is on
your forbidden list. If not enable logging by setting OPFLoggingEnabled to true in OPFService.config, restart the service and check logs for errors.

P.S. 

In case you decide to recompile the project and change OPFClientRecognitionKeyword you will need to change value of line 96 in dllmain.cpp to match the new keyword.

Links
------------
  * [Microsoft's documentation on installing password filters](https://msdn.microsoft.com/en-us/library/windows/desktop/ms721766(v=vs.85).aspx)
  * [Troy Hunt](https://twitter.com/troyhunt) 
  * [HaveIbeenPWNed](https://haveibeenpwned.com)
  * [OpenPasswordFilter](https://github.com/jephthai/OpenPasswordFilter)
