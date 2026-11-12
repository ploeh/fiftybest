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

## Authentication
Although authentication is currently not being used for anything, the [ambition
is to add features that make use of it](https://github.com/ploeh/fiftybest/issues/7).

### Dvelopment setup
Currently, GitHub authentication is implemented. For development purposes, you
will need to configure a GitHub OAuth application for your local development
environment:

1. Go to https://github.com/settings/developers
2. Click "OAuth Apps" in the left sidebar
3. Click "New OAuth App"
4. Fill in the form:
   - **Application name**: FiftyBest (or your preferred name)
   - **Homepage URL**: Your app's URL (e.g., `https://fiftybest.your-domain.com`)
   - **Authorization callback URL**: `https://your-domain.com/signin-github`
   (replace with your actual domain; for testing, you may use `localhost` and a
   port number; e.g. `http://localhost:5298/signin-github`)
5. Click "Register application"
6. On the next page, note the **Client ID** 
7. Click "Generate a new client secret" and save the **Client Secret**
   (you won't be able to see it again)
8. Add these values to your `appsettings.json`:
   ```json
   "Authentication": {
     "GitHub": {
       "ClientId": "your-client-id-here",
       "ClientSecret": "your-client-secret-here"
     }
   }
   ```