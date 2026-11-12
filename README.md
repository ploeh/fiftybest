# Fifty best restaurants browsable database

This is a web-based browsable database of the [fifty best restaurants in the
world](https://www.the50.com/). It contains historical data for each year the
awards list have been published, and enables users to browse the restaurants by
year, country, city, or name.

The database runs entirely in server-side memory, due to its small size. At
startup, the database is loaded from CSV files.

## Testing
Most views are fairly simple and contains little logic worth testing. Thus, the
test coverage is currently limited to the few areas in which more substantial
logic is present.

As a policy, if a bug is found, it should be reproduced by a test before it is
fixed.