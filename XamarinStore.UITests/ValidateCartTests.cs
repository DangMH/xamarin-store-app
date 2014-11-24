using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Xamarin.UITest;
using Xamarin.UITest.Android;
using Xamarin.UITest.Queries;

namespace XamarinStore.UITests
{
    [TestFixture]
    public class ValidateCartTests
    {
        protected AndroidApp _app;
        protected TestContext context;

        [SetUp]
        public void SetUp()
        {
            _app = Resources.ConfigureAndroidApp(Resources.ANDROID_APP_APK_FILEPATH);
            context = TestContext.CurrentContext;
        }

        [TearDown]
        public void ScreenCapture()
        {
            Resources.Screenshot(_app, context.Test.Name + "-" + context.Result.State.ToString());
        }

        [Test]
        public void ValidateAppLoadsWithEmptyCart()
        {
            // Navigate to Basket Cart
            Resources.WaitForElement(_app, Resources.BASKET_CART);
            _app.Tap(Resources.BASKET_CART);

            // Wait for app to switch views and validate that the empty basket text view is displayed
            Resources.WaitForElement(_app, Resources.BASKET_EMPTY);
        }

        [Test]
        public void ValidateAddItemToCart()
        {
            AppResult[] homeViewProducts = null,
                shopViewProductTitlePrice = null,
                shopViewProductSizeColor = null,
                cartViewProduct = null;

            // For each product available
            Resources.WaitForElement(_app, Resources.HOME_VIEW__PRODUCTS);
            homeViewProducts = _app.Query(Resources.HOME_VIEW__PRODUCTS);
            for (int i = 0; i < homeViewProducts.Length; i += 2)
            {
                // Tap product and add to basket
                Resources.ScrollDown(_app, homeViewProducts[i].Text);
                _app.Tap(c => c.Marked(homeViewProducts[i].Text));
                Resources.WaitForElement(_app, Resources.ADD_TO_BASKET);

                // Verify product title and product price found on main page
                shopViewProductTitlePrice = _app.Query(Resources.SHOP_VIEW_PRODUCT_TITLE_PRICE);
                Assert.AreEqual(homeViewProducts[i].Text, shopViewProductTitlePrice[0].Text);
                Assert.AreEqual(homeViewProducts[i + 1].Text, shopViewProductTitlePrice[1].Text);

                // Record shop view size and color
                shopViewProductSizeColor = _app.Query(Resources.SHOP_VIEW_PRODUCT_SIZE_COLOR);

                // Add product and navigate to Basket Cart and verify item is found
                _app.Tap(Resources.ADD_TO_BASKET);
                _app.Tap(Resources.BASKET_CART);
                Resources.WaitForElement(_app, Resources.CHECKOUT);

                // Validate product is in the cart
                cartViewProduct = _app.Query(Resources.CHECKOUT_VIEW_PRODUCT);
                Assert.AreEqual(homeViewProducts[i].Text, cartViewProduct[0].Text);
                Assert.AreEqual(homeViewProducts[i + 1].Text, cartViewProduct[1].Text);
                Assert.AreEqual(shopViewProductSizeColor[0].Text, cartViewProduct[2].Text);
                Assert.AreEqual(shopViewProductSizeColor[1].Text, cartViewProduct[3].Text);

                // Remove product from cart
                _app.DragCoordinates(cartViewProduct[0].Rect.X, cartViewProduct[0].Rect.Y, cartViewProduct[0].Rect.X + 500.0f, cartViewProduct[0].Rect.Y);

                // Verify empty basket text view is displayed
                Resources.WaitForElement(_app, Resources.BASKET_EMPTY);

                // Return to main screen
                _app.Tap(Resources.HOME);
                Resources.WaitForElement(_app, Resources.HOME_VIEW__PRODUCTS);
            }
        }
    }
}
