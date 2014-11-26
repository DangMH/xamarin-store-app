using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.UITest;
using Xamarin.UITest.Android;
using Xamarin.UITest.iOS;
using Xamarin.UITest.Queries;

namespace XamarinStore.UITests
{
    /// <summary>
    /// Contains global Resources and Macros for the UI tests
    /// </summary>
    public class Resources
    {
        /// <summary>
        /// Full filepath of the source directory
        /// </summary>
        public const string SOURCE_DIR = @"D:\Users\Michael\Source\Repos\xamarin-store-app\";
        /// <summary>
        /// Full filepath of the screenshots directory
        /// </summary>
        public const string SCREENSHOT_DIR = SOURCE_DIR + @"XamarinStore.UITests\Screenshots\";
        /// <summary>
        /// Full filepath of the apk file to be tested
        /// </summary>
        public const string ANDROID_APP_APK_FILEPATH = SOURCE_DIR + @"XamarinStore.Droid\bin\x86\Release\com.xamarin.XamStore-Signed.apk";
        /// <summary>
        /// Full filepath for the iOS app
        /// </summary>
        public const string IOS_APP_FILEPATH = SOURCE_DIR + @"XamarinStore.iOS\bin\iPhoneSimulator\Release\XamarinStoreiOS.exe";
        /// <summary>
        /// Device serial of the Nexus 4 Emulator
        /// </summary>
        public const string NEXUS_4_DEVICE_SERIAL = "10.71.34.101:5555";
        /// <summary>
        /// Extension of the screenshots, i.e. ".png"
        /// </summary>
        public const string SCREENSHOT_EXT = ".png";
        /// <summary>
        /// Email of the Xamarin account
        /// </summary>
        public const string TEST_ACCOUNT_EMAIL = "SampleEmail@Domain.Com";
        /// <summary>
        /// Password of the Xamarin account
        /// </summary>
        public const string TEST_ACCOUNT_PASSWORD = "SamplePassword";
        /// <summary>
        /// Maximum number of drag attempts before the ScrollDown macro returns
        /// </summary>
        public const int MAX_DRAG_ATTEMPTS = 30;

        /// <summary>
        /// Set of inputs for the shipping info1
        /// </summary>
        public static string[] TEST_SHIPPING_INFO1 = {
                                                        "SampleFirstName",  // [0] = First Name*            *Required
                                                        "SampleLastName",   // [1] = Last Name*             **Required and Verified
                                                        "123-456-7890",     // [2] = Phone Number*          ***Required and Verified in usa
                                                        "123 ST NE",        // [3] = Street Address 1*
                                                        "APT 456",          // [4] = Street Address 2
                                                        "Fake City",        // [5] = City*
                                                        "12345",            // [6] = Postal Code*
                                                    };
        /// <summary>
        /// Set of inputs for the shipping info2
        /// </summary>
        public static string[] TEST_SHIPPING_INFO2 = {
                                                        "usa",    // [0] = Country**
                                                        "Fake State"        // [1] = State***
                                                    };
        /// <summary>
        /// Set of queries of error elements expected for shipping info1
        /// </summary>
        public static Func<AppQuery, AppQuery>[] TEST_INVALID_SHIPPING_INFO1 = {
                                                                c => c.Marked("First name is required"),
                                                                c => c.Marked("Last name is required"),
                                                                c => c.Marked("Phone number is required"),
                                                                c => c.Marked("Address is required"),
                                                                null,
                                                                c => c.Marked("City is required"),
                                                                c => c.Marked("ZipCode is required")
                                                            };
        /// <summary>
        /// Set of queries of error elements expected for shipping info2
        /// </summary>
        public static Func<AppQuery, AppQuery>[] TEST_INVALID_SHIPPING_INFO2 = {
                                                                c => c.Marked("Country is required"),
                                                                c => c.Marked("State is required")
                                                            };

        /// <summary>
        /// Query for the basket cart navigation element in all views
        /// </summary>
        public static readonly Func<AppQuery, AppQuery> ALL_VIEW_BASKET_CART = c => c.Marked("cart_menu_item");
        /// <summary>
        /// Query for the "Your basket is empty" element in the Cart View
        /// </summary>
        public static readonly Func<AppQuery, AppQuery> CART_VIEW_BASKET_EMPTY = c => c.Text("Your basket is empty");
        /// <summary>
        /// Query for the email text field element of Checkout View1
        /// </summary>
        public static readonly Func<AppQuery, AppQuery> CHECKOUT_VIEW1_EMAIL = c => c.Marked("email");
        /// <summary>
        /// Query for the password field element in the Checkout View1
        /// </summary>
        public static readonly Func<AppQuery, AppQuery> CHECKOUT_VIEW1_PASSWORD = c => c.Marked("password");
        /// <summary>
        /// Query for the sign in button element in the Checkout View1
        /// </summary>
        public static readonly Func<AppQuery, AppQuery> CHECKOUT_VIEW1_SIGN_IN = c => c.Button("signInBtn");
        /// <summary>
        /// Query for the place order button element of Checkout View2
        /// </summary>
        public static readonly Func<AppQuery, AppQuery> CHECKOUT_VIEW2_PLACE_ORDER = c => c.Button("PlaceOrder");
        /// <summary>
        /// Query for the set of shipping info 1 elements in Checkout View2
        /// </summary>
        public static readonly Func<AppQuery, AppQuery> CHECKOUT_VIEW2_SHIPPING_INFORMATION1 = c => c.Class("EditText");                                     // [0-6] = SHIPPING_INFO1[0-6]
        /// <summary>
        /// Query for the set of shipping info 2 elements in Checkout View2
        /// </summary>
        public static readonly Func<AppQuery, AppQuery> CHECKOUT_VIEW2_SHIPPING_INFORMATION2 = c => c.Class("AutoCompleteTextView");                         // [0-1] = SHIPPING_INFO2[0-1]
        /// <summary>
        /// Query for the Order Complete element in Checkout View3
        /// </summary>
        public static readonly Func<AppQuery, AppQuery> CHECKOUT_VIEW3_ORDER_COMPLETE = c => c.Marked("Order Complete");
        /// <summary>
        /// Query for the home element
        /// </summary>
        public static readonly Func<AppQuery, AppQuery> HOME_VIEW = c => c.Marked("home");
        /// <summary>
        /// Query for the set of product elements in Home View
        /// </summary>
        public static readonly Func<AppQuery, AppQuery> HOME_VIEW__PRODUCTS = c => c.All().Marked("NoResourceEntry-1610612737").Descendant("TextView");     // [2*i] = title, [2*i+1] = the price
        /// <summary>
        /// Query for the add to basket button element in the Product View
        /// </summary>
        public static readonly Func<AppQuery, AppQuery> PRODUCT_VIEW_ADD_TO_BASKET = c => c.Button("addToBasket");
        /// <summary>
        /// Query for the Checkout button element in the Product View
        /// </summary>
        public static readonly Func<AppQuery, AppQuery> PRODUCT_VIEW_CHECKOUT = c => c.Button("Checkout");
        /// <summary>
        /// Query for the dropdown element in the Product View
        /// </summary>
        public static readonly Func<AppQuery, AppQuery> PRODUCT_VIEW_DROPDOWN = c => c.Class("DropDownListView");
        /// <summary>
        /// Query for the items of a dropdown element in the Product View
        /// </summary>
        public static readonly Func<AppQuery, AppQuery> PRODUCT_VIEW_DROPDOWN_ITEMS = c => c.Class("CheckedTextView");
        /// <summary>
        /// Query for the dropdown elements in the Product View
        /// </summary>
        public static readonly Func<AppQuery, AppQuery> PRODUCT_VIEW_DROPDOWNS = c => c.Class("Spinner");                                                   // [0] = size, [1] = color
        /// <summary>
        /// Query for the product size and color elements in the Product View
        /// </summary>
        public static readonly Func<AppQuery, AppQuery> PRODUCT_VIEW_PRODUCT_SIZE_COLOR = c => c.Marked("descriptionLayout").Descendant("CheckedTextView"); // [0] = size, [1] = color
        /// <summary>
        /// Query for the product title and price elements in the Product View
        /// </summary>
        public static readonly Func<AppQuery, AppQuery> PRODUCT_VIEW_PRODUCT_TITLE_PRICE = c => c.Marked("descriptionLayout").Descendant("TextView");       // [0] = title, [1] = price 
        /// <summary>
        /// Query for the product elements in the Product View
        /// </summary>
        public static readonly Func<AppQuery, AppQuery> PRODUCT_VIEW_PRODUCTS = c => c.Marked("SwipeContent").Descendant("TextView");                       // [4*i] = title, [4*i+1] = price, [4*i+2] = size, [4*i+3] = color

        // Make Resources un-instantiable
        /// <summary>
        /// Private constructor.  The class is strictly for global variables and Macros.
        /// </summary>
        private Resources()
        {

        }

        /// <summary>
        /// Macro that configures the device to the specified Android APK and device serial
        /// </summary>
        /// <param name="androidAPKFilepath">Full filepath to the android app APK</param>
        /// <param name="deviceSerial">Device serial of the emulator or device</param>
        public static AndroidApp ConfigureAndroidApp(string androidAPKFilepath, string deviceSerial)
        {
            if (null != deviceSerial)
            {
                return ConfigureApp.Android.ApkFile(androidAPKFilepath).DeviceSerial(deviceSerial).EnableLocalScreenshots().StartApp();
            }
            else
            {
                return ConfigureApp.Android.ApkFile(androidAPKFilepath).EnableLocalScreenshots().StartApp();
            }
        }

        /// <summary>
        /// Macro that configures the device to the specified Android APK
        /// </summary>
        /// <param name="androidAPKFilepath">Full filepath to the android app APK</param>
        public static AndroidApp ConfigureAndroidApp(string androidAPKFilepath)
        {
            return ConfigureAndroidApp(androidAPKFilepath, null);
        }
        /// <summary>
        /// Macro that configures the device to the specified iOS App
        /// </summary>
        /// <param name="iosAppFilepath">Full filepath to the iOS App</param>
        public static iOSApp ConfigureIOSApp(string iosAppFilepath)
        {
            return ConfigureApp.iOS.AppBundle(iosAppFilepath).EnableLocalScreenshots().StartApp();
        }

        /// <summary>
        /// Macro to take a screenshot that will be stored at SCREENSHOT_DIR.  Naming will be tagged with ascending decimal order with no overwriting.
        /// </summary>
        /// <param name="app">App to call the Macro on</param>
        /// <param name="filename">Filename of the screenshot</param>
        public static void Screenshot(IApp app, string filename)
        {
            FileInfo fi = app.Screenshot(filename);

            int i = 0;
            while (File.Exists(SCREENSHOT_DIR + filename + i + SCREENSHOT_EXT))
            {
                ++i;
            }

            fi.CopyTo(SCREENSHOT_DIR + filename + i + SCREENSHOT_EXT);
        }

        /// <summary>
        /// Macro to wait for the specified element to appear
        /// </summary>
        /// <param name="app">App to call the Macro on</param>
        /// <param name="query">Query for the element to wait for</param>
        public static void WaitForElement(IApp app, Func<AppQuery, AppQuery> query)
        {
            try
            {
                app.WaitForElement(query);
            }
            catch (TimeoutException te)
            {
                Assert.Fail(te.ToString());
            }
        }

        /// <summary>
        /// Macro to wait for the specified element to disappear
        /// </summary>
        /// <param name="app">App to call the Macro on</param>
        /// <param name="query">Query for the element to wait for</param>
        public static void WaitForNoElement(IApp app, Func<AppQuery, AppQuery> query)
        {
            try
            {
                app.WaitForNoElement(query);
            }
            catch (TimeoutException te)
            {
                Assert.Fail(te.ToString());
            }
        }

        /// <summary>
        /// Macro to scroll down until the element marked is found or MAX_DRAG_ATTEMPTS is reached
        /// </summary>
        /// <param name="app">App to call the Macro on</param>
        /// <param name="marked">String identifier that marks the element</param>
        public static void ScrollDown(IApp app, string marked)
        {
            int attempts = MAX_DRAG_ATTEMPTS;

            while (0 == app.Query(c => c.Marked(marked)).Length && 0 < attempts--)
            {
                app.DragCoordinates(0.0f, 150f, 0.0f, 0.0f);
            }
        }

        /// <summary>
        /// Macro that will set the text field of the element marked
        /// </summary>
        /// <param name="app">App to call the Macro on</param>
        /// <param name="marked">String identifier that marks the element</param>
        /// <param name="text">Text to enter</param>
        public static void EnterText(IApp app, string marked, string text)
        {
            app.Query(c => c.Marked(marked).Invoke("setText", text));
        }

        /// <summary>
        /// Macro to tap the query element and wait for the expectedQuery element
        /// </summary>
        /// <param name="app">App to call the Macro on</param>
        /// <param name="query">Query for the element to tap</param>
        /// <param name="expectedQuery">Query for the element to wait for</param>
        public static void TapAndWaitForElement(IApp app, Func<AppQuery, AppQuery> query, Func<AppQuery, AppQuery> expectedQuery)
        {
            if (null != query)
            {
                app.Tap(query);
            }
            else
            {
                app.Back();
            }

            if (null != expectedQuery)
            {
                Resources.WaitForElement(app, expectedQuery);
            }
        }
    }
}
