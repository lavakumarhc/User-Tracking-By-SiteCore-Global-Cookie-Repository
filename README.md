# User-Tracking-By-SiteCore-Global-Cookie-Repository

Identify previously registered user using Sitecore Global Cookie User had previously logged in from the same browser Display some message to the user who has previously logged in from this browser, but has not logged in recently. This recent period can be specified while configuring the personalization rule.

Identification of previously registered user but not logged in for long time has been identified using Sitecore Global Cookie as below.

1. Find the SC_ANALYTICS_GLOBAL_COOKIE value for a registered user as from the browser.
2. Respective Contact is present in the mongodb for registered user identified by an identifier.
3. Create a Rule under Security Group. Rule name : Where the Current user is anonymous but previously registered user not logged in from number of days.
