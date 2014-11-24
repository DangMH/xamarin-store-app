using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.UITest;
using Xamarin.UITest.Android;
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
        public const string ANDROID_APP_APK_FILEPATH = SOURCE_DIR + @"XamarinStore.Droid\com.xamarin.XamStore.apk";    
        public const string NEXUS_4_DEVICE_SERIAL = "10.71.34.101:5555";
        public const string SCREENSHOT_EXT = ".png";
        public const int DEFAULT_QUERY_WAIT_TIME = 100;

        public static readonly Func<AppQuery, AppQuery> BASKET_CART = c => c.Marked("cart_menu_item");
        public static readonly Func<AppQuery, AppQuery> BASKET_EMPTY = c => c.Text("Your basket is empty");
        public static readonly Func<AppQuery, AppQuery> HOME_VIEW__PRODUCTS = c => c.All().Marked("NoResourceEntry-1610612737").Descendant("TextView");     // [2*i] = title, [2*i+1] = the price
        public static readonly Func<AppQuery, AppQuery> SHOP_VIEW_PRODUCT_TITLE_PRICE = c => c.Marked("descriptionLayout").Descendant("TextView");          // [0] = title, [1] = price 
        public static readonly Func<AppQuery, AppQuery> SHOP_VIEW_PRODUCT_SIZE_COLOR = c => c.Marked("descriptionLayout").Descendant("CheckedTextView");    // [0] = size, [1] = color
        public static readonly Func<AppQuery, AppQuery> ADD_TO_BASKET = c => c.Button("addToBasket");
        public static readonly Func<AppQuery, AppQuery> CHECKOUT = c => c.Button("Checkout");
        public static readonly Func<AppQuery, AppQuery> CHECKOUT_VIEW_PRODUCT = c => c.Marked("SwipeContent").Descendant("TextView");                       // [4*i] = title, [4*i+1] = price, [4*i+2] = size, [4*i+3] = color
        public static readonly Func<AppQuery, AppQuery> HOME = c => c.Marked("home");

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

        public static void Screenshot(AndroidApp app, string filename)
        {
            FileInfo fi = app.Screenshot(filename);

            int i = 0;
            while (File.Exists(SCREENSHOT_DIR + filename + i + SCREENSHOT_EXT))
            {
                ++i;
            }

            fi.CopyTo(SCREENSHOT_DIR + filename + i + SCREENSHOT_EXT);
        }

        public static void WaitForElement(AndroidApp app, Func<AppQuery, AppQuery> query)
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

        public static void ScrollDown(AndroidApp app, string id)
        {
            while(0 == app.Query(c => c.Marked(id)).Length)
            {
                app.DragCoordinates(0.0f, 150f, 0.0f, 0.0f);
            }
        }
    }
}
