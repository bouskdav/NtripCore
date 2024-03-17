# NtripCore

This is a project of Jakub Koutny and me to learn a bit about NTRIP protocols and to have a NTRIP caster suitable to all our needs. Some points that were taken into mind during initial development:
- it must be compatible with DJI drones (we must solve some factors that prevent establishing successfull connection in DJI Pilot app)
- it must be lightweight, so it can be run onsite (ideally on raspberry pi)
- it must accept data from RTKBase (https://github.com/Stefal/rtkbase/) -> eventually can run on the same machine

Actually our main goal is the compatibility layer for DJI drones.

Some other features:
- aggregates source tables from multiple sources (e.g. our basegnss.local, rtk2go.com, CZEPOS, etc.)
- allows "nearest" station feature

Additional features yet to be implemented / TODO:
- GUI
- Automatically do cleanup (e.g. disconnect from paid streams)
- comment a code a bit
- dig a bit more into NTRIP protocol, so we can support more sources
- do proper handling of NTRIP rev 1 and rev 2 protocols