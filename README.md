# NtripCore

## NTRIP Caster written in .NET Core

This is a project of Jakub Koutny and me to learn a bit about NTRIP protocols and to have a NTRIP caster suitable to all our needs. Some points that were taken into mind during initial development:
- it must be compatible with DJI drones (we must solve some factors that prevent establishing successfull connection in DJI Pilot app)
- it must be lightweight, so it can be run onsite (ideally on raspberry pi)
- it must accept data from RTKBase (https://github.com/Stefal/rtkbase/) -> eventually can run on the same machine

Actually our main goal is the compatibility layer for DJI drones.

## Some other features:
- aggregates source tables from multiple sources (e.g. our basegnss.local, rtk2go.com, CZEPOS, etc.)
- allows "nearest" station feature (done by recalculation everytime it receives GPGGA message from client - this option is enabled by selecting NTC_Nearest mountpoint)
- properly handles NTRIP rev 1 and rev 2 protocols (rev 2 is usually paid feature, not here :))
- confirmed working with DJI M300 RTK (will add some numbers later)

## Additional features yet to be implemented / TODO:
- GUI
- Automatically do cleanup (e.g. disconnect from paid streams when no one want's it anymore)
- comment source code a bit
- dig a bit more into NTRIP protocol, so we can support more sources

## Some notes:
- DJI is quite eager in sending NMEA GPGGA messages - it send a message almost every second
- DJI is also quite eager in establishing connections - you should expect several connections per drone
- DJI is quite sensitive for response formatting
- User-Agent for M300 is NTRIP DJI_CSDK_NTRIP_CLIENT/1.4
- uses only a small resources (during debug and one rover it was like 25 MB)