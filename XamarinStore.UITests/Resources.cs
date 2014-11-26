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
    /// Contains global resources for unit tests
    /// </summary>
    public class Resources
    {
        public const string SOURCE_DIR = @"D:\Users\Michael\Source\Repos\xamarin-store-app\";
        public const string SCREENSHOT_DIR = SOURCE_DIR + @"XamarinStore.UITests\Screenshots\";
        public const string ANDROID_APP_APK_FILEPATH = SOURCE_DIR + @"XamarinStore.Droid\bin\x86\Release\com.xamarin.XamStore-Signed.apk";
        public const string IOS_APP_APP_FILEPATH = SOURCE_DIR + @"XamarinStore.iOS\bin\iPhoneSimulator\Release\XamarinStoreiOS.exe";
        public const string NEXUS_4_DEVICE_SERIAL = "10.71.34.101:5555";
        public const string SCREENSHOT_EXT = ".png";
        public const string TEST_ACCOUNT_EMAIL = "SampleEmail@Domain.Com";
        public const string TEST_ACCOUNT_PASSWORD = "SamplePassword";

        public static string[] TEST_SHIPPING_INFO1 = {
                                                        "SampleFirstName",  // [0] = First Name*            *Required
                                                        "SampleLastName",   // [1] = Last Name*             **Required and Verified
                                                        "123-456-7890",     // [2] = Phone Number*          ***Required and Verified in usa
                                                        "123 ST NE",        // [3] = Street Address 1*
                                                        "APT 456",          // [4] = Street Address 2
                                                        "Fake City",        // [5] = City*
                                                        "12345",            // [6] = Postal Code*
                                                    };
        public static string[] TEST_SHIPPING_INFO2 = {
                                                        "usa",    // [0] = Country**
                                                        "Fake State"        // [1] = State***
                                                    };
        public static Func<AppQuery, AppQuery>[] TEST_INVALID_SHIPPING_INFO1 = {
                                                                c => c.Marked("First name is required"),
                                                                c => c.Marked("Last name is required"),
                                                                c => c.Marked("Phone number is required"),
                                                                c => c.Marked("Address is required"),
                                                                null,
                                                                c => c.Marked("City is required"),
                                                                c => c.Marked("ZipCode is required")
                                                            };
        public static Func<AppQuery, AppQuery>[] TEST_INVALID_SHIPPING_INFO2 = {
                                                                c => c.Marked("Country is required"),
                                                                c => c.Marked("State is required")
                                                            };

        public static readonly Func<AppQuery, AppQuery> ALL_VIEW_BASKET_CART = c => c.Marked("cart_menu_item");
        public static readonly Func<AppQuery, AppQuery> CART_VIEW_BASKET_EMPTY = c => c.Text("Your basket is empty");
        public static readonly Func<AppQuery, AppQuery> CHECKOUT_VIEW1_EMAIL = c => c.Marked("email");
        public static readonly Func<AppQuery, AppQuery> CHECKOUT_VIEW1_SIGN_IN = c => c.Button("signInBtn");
        public static readonly Func<AppQuery, AppQuery> CHECKOUT_VIEW2_PASSWORD = c => c.Marked("password");
        public static readonly Func<AppQuery, AppQuery> CHECKOUT_VIEW2_PLACE_ORDER = c => c.Button("PlaceOrder");
        public static readonly Func<AppQuery, AppQuery> CHECKOUT_VIEW2_SHIPPING_INFORMATION1 = c => c.Class("EditText");                                     // [0-6] = SHIPPING_INFO1[0-6]
        public static readonly Func<AppQuery, AppQuery> CHECKOUT_VIEW2_SHIPPING_INFORMATION2 = c => c.Class("AutoCompleteTextView");                         // [0-1] = SHIPPING_INFO2[0-1]
        public static readonly Func<AppQuery, AppQuery> CHECKOUT_VIEW3_ORDER_COMPLETE = c => c.Marked("Order Complete");
        public static readonly Func<AppQuery, AppQuery> HOME_VIEW = c => c.Marked("home");
        public static readonly Func<AppQuery, AppQuery> HOME_VIEW__PRODUCTS = c => c.All().Marked("NoResourceEntry-1610612737").Descendant("TextView");     // [2*i] = title, [2*i+1] = the price
        public static readonly Func<AppQuery, AppQuery> PRODUCT_VIEW_ADD_TO_BASKET = c => c.Button("addToBasket");
        public static readonly Func<AppQuery, AppQuery> PRODUCT_VIEW_CHECKOUT = c => c.Button("Checkout");
        public static readonly Func<AppQuery, AppQuery> PRODUCT_VIEW_DROPDOWN = c => c.Class("DropDownListView");
        public static readonly Func<AppQuery, AppQuery> PRODUCT_VIEW_DROPDOWN_ITEMS = c => c.Class("CheckedTextView");
        public static readonly Func<AppQuery, AppQuery> PRODUCT_VIEW_DROPDOWNS = c => c.Class("Spinner");                                                   // [0] = size, [1] = color
        public static readonly Func<AppQuery, AppQuery> PRODUCT_VIEW_PRODUCT_SIZE_COLOR = c => c.Marked("descriptionLayout").Descendant("CheckedTextView"); // [0] = size, [1] = color
        public static readonly Func<AppQuery, AppQuery> PRODUCT_VIEW_PRODUCT_TITLE_PRICE = c => c.Marked("descriptionLayout").Descendant("TextView");       // [0] = title, [1] = price 
        public static readonly Func<AppQuery, AppQuery> PRODUCT_VIEW_PRODUCTS = c => c.Marked("SwipeContent").Descendant("TextView");                       // [4*i] = title, [4*i+1] = price, [4*i+2] = size, [4*i+3] = color

        // Make Resources un-instantiable
        private Resources()
        {

        }

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

        public static AndroidApp ConfigureAndroidApp(string androidAPKFilepath)
        {
            return ConfigureAndroidApp(androidAPKFilepath, null);
        }
        public static iOSApp ConfigureIOSApp(string iosAPPFilepath)
        {
            return ConfigureApp.iOS.AppBundle(iosAPPFilepath).EnableLocalScreenshots().StartApp();
        }

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

        public static void ScrollDown(IApp app, string marked)
        {
            while (0 == app.Query(c => c.Marked(marked)).Length)
            {
                app.DragCoordinates(0.0f, 150f, 0.0f, 0.0f);
            }
        }

        public static void EnterText(IApp app, string marked, string text)
        {
            app.Query(c => c.Marked(marked).Invoke("setText", text));
        }

        public static void TapAndWaitForElement(IApp app, Func<AppQuery, AppQuery> query, Func<AppQuery, AppQuery> expectedQuery)
        {
            if(null != query)
            {
                app.Tap(query);
            }
            else
            {
                app.Back();
            }

            if(null != expectedQuery)
            {
                Resources.WaitForElement(app, expectedQuery);
            }
        }
    }
}
