using System.Web;
using Sitecore.Diagnostics;
using Sitecore.Rules;
using Sitecore.Rules.Conditions;
using System;
using Sitecore.Analytics.Data;
using Sitecore.Analytics.Tracking;
using Sitecore.Configuration;
using Sitecore.Analytics.Model;
using System.Collections.Generic;

namespace SitecoreGlobalCookieUserTracking.Extensibility.Rules
{
    /// <summary>
    /// Defines the item readable condition class.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class AnonymousButLoggedInCondition<T> : OperatorCondition<T> where T : RuleContext
    {
        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>The value.</value>
        public string Value
        {
            get;
            set;
        }

        /// <summary>
        /// Executes the specified rule context.
        /// </summary>
        /// <param name="ruleContext">The rule context.</param>
        /// <returns>
        /// 	<c>True</c>, if the condition succeeds, otherwise <c>false</c>.
        /// </returns>
        protected override bool Execute(T ruleContext)
        {
            // Get the Global Cookie

            HttpCookie globalCookie = HttpContext.Current.Request.Cookies["sc_analytics_global_cookie"];

            
            ContactRepository contactRepository = Factory.CreateObject("tracking/contactRepository", true) as ContactRepository;
            ContactManager contactManager = Factory.CreateObject("tracking/contactManager", true) as ContactManager;

            Assert.IsNotNull(contactRepository, "contactRepository");
            Assert.IsNotNull(contactManager, "contactManager");

            // Number of days which user has not logged in
            int value;
            if (!int.TryParse(this.Value, out value))
            {
                Log.Debug(string.Format("Specified number [{0}] was not a valid Number", this.Value));
                return false;
            }
            if (globalCookie != null)
            {

                // Find the Cookie Value
                string cookieValue = globalCookie.Value.Substring(0, 32);

                // Convert the Cookie value to Guid 
                Guid contactid = Guid.Parse(cookieValue);
                
                // Get the Contact using Golable Cookie
                Contact contact = contactRepository.LoadContactReadOnly(contactid);
                if (contact != null)
                {
                    // Identify the the Known contact
                    if (contact.Identifiers.IdentificationLevel == ContactIdentificationLevel.Known)
                    {
                        DateTime currentDateTime = DateTime.Now;
                        DateTime pastDateTime = currentDateTime.AddDays(-value);

                        //Find out if there are any interaction from past date
                        IEnumerable<IInteractionData> lastInteraction = contactRepository.LoadHistoricalInteractions(contactid, 1, pastDateTime, currentDateTime);
                        
                        if (lastInteraction.GetEnumerator().Current == null)
                        {
                            return true;
                        }

                    }
                }


            }

            return false;
        }

    }
}