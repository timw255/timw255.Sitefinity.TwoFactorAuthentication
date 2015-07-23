# Two Factor Authentication

This is a custom module that adds basic two-factor authentication to Telerik Sitefinity. It's not entirely integrated since, well, I didn't feel like it. :P

## Authy

It relies on [Authy](https://www.authy.com/) to provide and validate the tokens. You must create an account and set up an Authy app to use this as is.

Once that's done, grab the Api Key from your Authy dashboard and place it in the configuration at: Administration -> Settings -> Advanced -> TwoFactorAuthentication.

## Enabling Two-factor Authentication

After installing the module, you must change the "issuer" in your web.config to make Sitefinity use TFA instead of the default login. Here's an example:

```xml
<wsFederation passiveRedirectEnabled="true" issuer="http://<yoursite>/TFA/Authenticate/SWT" realm="http://localhost" requireHttps="false" />
```

## Where does it work?

This will work for backend authentication out of the box, for frontend logins, please use the "Two Factor Login" widget that's installed in the "Two Factor Authentication" section of the toolbox.

## To "Opt In" a User

This module adds a new field (Authy Id) to the Basic profile. If a value is set for a user, two-factor authentication is enforced. If not, username and password is all that's required.