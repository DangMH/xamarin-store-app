﻿using NUnit.Framework;
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
    /// <summary>
    /// Class containing the UI tests for the Xamarin store.  Has dependencies on the Resources class.
    /// </summary>
    [TestFixture]
    public class XamarinStoreUITests
    {
        public enum DeviceSupported
        {
            ANDROID,
            IOS,
            WINDOWSPHONE    // future support
        }

        /// <summary>
        /// App device to run the tests on.
        /// </summary>
        protected IApp _app;
        /// <summary>
        /// Context of the current test.
        /// </summary>
        protected TestContext context;
        /// <summary>
        /// Set to run the device to support for the current test pass.
        /// </summary>
        protected DeviceSupported device = DeviceSupported.ANDROID;
        /// <summary>
        /// Set to run Repl() at the beginning of each test.
        /// </summary>
        protected bool debug = false;

        /// <summary>
        /// Set up method to configure the app and test context
        /// </summary>
        [SetUp]
        public void SetUp()
        {
            switch (device)
            {
                case DeviceSupported.ANDROID:
                    _app = Resources.ConfigureAndroidApp(Resources.ANDROID_APP_APK_FILEPATH);
                    break;
                case DeviceSupported.IOS:
                    _app = Resources.ConfigureIOSApp(Resources.IOS_APP_FILEPATH);
                    break;
                default:
                    break;
            }

            context = TestContext.CurrentContext;

            if (debug)
            {
                _app.Repl();
            }
        }

        /// <summary>
        /// Tear down method to screencapture before exiting
        /// </summary>
        [TearDown]
        public void ScreenCapture()
        {
            Resources.Screenshot(_app, context.Test.Name + "-" + context.Result.State.ToString());
        }

        /// <summary>
        /// Test case that verifies that the app will load with an empty cart.
        /// </summary>
        [Test]
        public void ValidateAppLoadsWithEmptyCart()
        {
            // Load up app
            Resources.WaitForElement(_app, Resources.ALL_VIEW_BASKET_CART);

            // Navigate to Basket Cart and validate that the empty basket text view is displayed
            Resources.TapAndWaitForElement(_app, Resources.ALL_VIEW_BASKET_CART, Resources.CART_VIEW_BASKET_EMPTY);
        }

        /// <summary>
        /// Test case that adds each product individually and verifies the correct elements are displayed in the cart.
        /// </summary>
        [Test]
        public void ValidateInCartAddAllProducts()
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
                // Navigate to Product View and verify navigation
                Resources.ScrollDown(_app, homeViewProducts[i].Text);
                Resources.TapAndWaitForElement(_app, c => c.Marked(homeViewProducts[i].Text), Resources.PRODUCT_VIEW_ADD_TO_BASKET);

                ValidateProductShopView(homeViewProducts[i].Text, homeViewProducts[i + 1].Text);

                // Record shop view size and color
                shopViewProductTitlePrice = _app.Query(Resources.PRODUCT_VIEW_PRODUCT_TITLE_PRICE);
                shopViewProductSizeColor = _app.Query(Resources.PRODUCT_VIEW_PRODUCT_SIZE_COLOR);

                // Add product and navigate to Basket Cart View and verify navigation
                _app.Tap(Resources.PRODUCT_VIEW_ADD_TO_BASKET);
                Resources.TapAndWaitForElement(_app, Resources.ALL_VIEW_BASKET_CART, Resources.PRODUCT_VIEW_CHECKOUT);

                ValidateProductCartView(shopViewProductTitlePrice[0].Text, shopViewProductTitlePrice[1].Text, shopViewProductSizeColor[0].Text, shopViewProductSizeColor[1].Text, 0);

                // Remove product from cart
                cartViewProduct = _app.Query(Resources.PRODUCT_VIEW_PRODUCTS);
                _app.DragCoordinates(cartViewProduct[0].Rect.X, cartViewProduct[0].Rect.Y, cartViewProduct[0].Rect.X + 500.0f, cartViewProduct[0].Rect.Y);

                // Verify empty basket text view is displayed
                Resources.WaitForElement(_app, Resources.CART_VIEW_BASKET_EMPTY);

                Resources.TapAndWaitForElement(_app, Resources.HOME_VIEW, Resources.HOME_VIEW__PRODUCTS);
            }
        }

        /// <summary>
        /// Test case that adds each product in each size variation individually and verifies the correct elements are displayed in the cart.
        /// </summary>
        [Test]
        public void ValidateInCartChangeAllSizesAllProducts()
        {
            AppResult[] homeViewProducts = null,
                shopViewProductTitlePrice = null,
                shopViewProductSizeColor = null,
                productSizes = null,
                cartViewProduct = null;

            // For each product available
            Resources.WaitForElement(_app, Resources.HOME_VIEW__PRODUCTS);
            homeViewProducts = _app.Query(Resources.HOME_VIEW__PRODUCTS);
            for (int i = 0; i < homeViewProducts.Length; i += 2)
            {
                // Navigate to Product View and verify navigation
                Resources.ScrollDown(_app, homeViewProducts[i].Text);
                Resources.TapAndWaitForElement(_app, c => c.Marked(homeViewProducts[i].Text), Resources.PRODUCT_VIEW_ADD_TO_BASKET);

                ValidateProductShopView(homeViewProducts[i].Text, homeViewProducts[i + 1].Text);

                // Get the list of sizes
                Resources.TapAndWaitForElement(_app, c => c.Marked(_app.Query(Resources.PRODUCT_VIEW_DROPDOWNS)[0].Id), Resources.PRODUCT_VIEW_DROPDOWN);
                productSizes = _app.Query(Resources.PRODUCT_VIEW_DROPDOWN_ITEMS);
                Resources.TapAndWaitForElement(_app, null, Resources.PRODUCT_VIEW_ADD_TO_BASKET);

                // Return to main screen
                Resources.TapAndWaitForElement(_app, Resources.HOME_VIEW, Resources.HOME_VIEW__PRODUCTS);

                // Iterate through all sizes
                foreach (AppResult productSize in productSizes)
                {
                    // Navigate to Product View and verify navigation
                    Resources.ScrollDown(_app, homeViewProducts[i].Text);
                    Resources.TapAndWaitForElement(_app, c => c.Marked(homeViewProducts[i].Text), Resources.PRODUCT_VIEW_ADD_TO_BASKET);

                    // Modify the size
                    Resources.TapAndWaitForElement(_app, c => c.Marked(_app.Query(Resources.PRODUCT_VIEW_DROPDOWNS)[0].Id), Resources.PRODUCT_VIEW_DROPDOWN);
                    Resources.TapAndWaitForElement(_app, c => c.Marked(productSize.Text), Resources.PRODUCT_VIEW_ADD_TO_BASKET);

                    // Record shop view size and color
                    shopViewProductTitlePrice = _app.Query(Resources.PRODUCT_VIEW_PRODUCT_TITLE_PRICE);
                    shopViewProductSizeColor = _app.Query(Resources.PRODUCT_VIEW_PRODUCT_SIZE_COLOR);

                    // Add product and navigate to Basket Cart View and verify navigation
                    _app.Tap(Resources.PRODUCT_VIEW_ADD_TO_BASKET);
                    Resources.TapAndWaitForElement(_app, Resources.ALL_VIEW_BASKET_CART, Resources.PRODUCT_VIEW_CHECKOUT);

                    ValidateProductCartView(shopViewProductTitlePrice[0].Text, shopViewProductTitlePrice[1].Text, shopViewProductSizeColor[0].Text, shopViewProductSizeColor[1].Text, 0);

                    // Remove product from cart
                    cartViewProduct = _app.Query(Resources.PRODUCT_VIEW_PRODUCTS);
                    _app.DragCoordinates(cartViewProduct[0].Rect.X, cartViewProduct[0].Rect.Y, cartViewProduct[0].Rect.X + 500.0f, cartViewProduct[0].Rect.Y);

                    // Verify empty basket text view is displayed
                    Resources.WaitForElement(_app, Resources.CART_VIEW_BASKET_EMPTY);

                    Resources.TapAndWaitForElement(_app, Resources.HOME_VIEW, Resources.HOME_VIEW__PRODUCTS);
                }
            }
        }

        /// <summary>
        /// Test case that adds each product in each color variation individually and verifies the correct elements are displayed in the cart.
        /// </summary>
        [Test]
        public void ValidateInCartChangeAllColorsAllProducts()
        {
            AppResult[] homeViewProducts = null,
                shopViewProductTitlePrice = null,
                shopViewProductSizeColor = null,
                productColors = null,
                cartViewProduct = null;

            // For each product available
            Resources.WaitForElement(_app, Resources.HOME_VIEW__PRODUCTS);
            homeViewProducts = _app.Query(Resources.HOME_VIEW__PRODUCTS);
            for (int i = 0; i < homeViewProducts.Length; i += 2)
            {
                // Navigate to Product View and verify navigation
                Resources.ScrollDown(_app, homeViewProducts[i].Text);
                Resources.TapAndWaitForElement(_app, c => c.Marked(homeViewProducts[i].Text), Resources.PRODUCT_VIEW_ADD_TO_BASKET);

                // Get the list of colors
                Resources.TapAndWaitForElement(_app, c => c.Marked(_app.Query(Resources.PRODUCT_VIEW_DROPDOWNS)[1].Id), Resources.PRODUCT_VIEW_DROPDOWN);
                productColors = _app.Query(Resources.PRODUCT_VIEW_DROPDOWN_ITEMS);
                Resources.TapAndWaitForElement(_app, null, Resources.PRODUCT_VIEW_ADD_TO_BASKET);

                // Return to main screen
                Resources.TapAndWaitForElement(_app, Resources.HOME_VIEW, Resources.HOME_VIEW__PRODUCTS);

                // Iterate through all colors
                foreach (AppResult productColor in productColors)
                {
                    // Navigate to Product View and verify navigation
                    Resources.ScrollDown(_app, homeViewProducts[i].Text);
                    Resources.TapAndWaitForElement(_app, c => c.Marked(homeViewProducts[i].Text), Resources.PRODUCT_VIEW_ADD_TO_BASKET);

                    // Modify the size
                    Resources.TapAndWaitForElement(_app, c => c.Marked(_app.Query(Resources.PRODUCT_VIEW_DROPDOWNS)[1].Id), Resources.PRODUCT_VIEW_DROPDOWN);
                    Resources.TapAndWaitForElement(_app, c => c.Marked(productColor.Text), Resources.PRODUCT_VIEW_ADD_TO_BASKET);

                    // Record shop view size and color
                    shopViewProductTitlePrice = _app.Query(Resources.PRODUCT_VIEW_PRODUCT_TITLE_PRICE);
                    shopViewProductSizeColor = _app.Query(Resources.PRODUCT_VIEW_PRODUCT_SIZE_COLOR);

                    // Add product and navigate to Basket Cart View and verify navigation
                    _app.Tap(Resources.PRODUCT_VIEW_ADD_TO_BASKET);
                    Resources.TapAndWaitForElement(_app, Resources.ALL_VIEW_BASKET_CART, Resources.PRODUCT_VIEW_CHECKOUT);

                    ValidateProductCartView(shopViewProductTitlePrice[0].Text, shopViewProductTitlePrice[1].Text, shopViewProductSizeColor[0].Text, shopViewProductSizeColor[1].Text, 0);

                    // Remove product from cart
                    cartViewProduct = _app.Query(Resources.PRODUCT_VIEW_PRODUCTS);
                    _app.DragCoordinates(cartViewProduct[0].Rect.X, cartViewProduct[0].Rect.Y, cartViewProduct[0].Rect.X + 500.0f, cartViewProduct[0].Rect.Y);

                    // Verify empty basket text view is displayed
                    Resources.WaitForElement(_app, Resources.CART_VIEW_BASKET_EMPTY);

                    Resources.TapAndWaitForElement(_app, Resources.HOME_VIEW, Resources.HOME_VIEW__PRODUCTS);
                }
            }
        }

        /// <summary>
        /// Test case that adds each product twice and verifies that the two items can be removed successfully individually.
        /// </summary>
        [Test]
        public void ValidateInCartDoubleAddAllProducts()
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
                // Navigate to Product View and verify navigation
                Resources.ScrollDown(_app, homeViewProducts[i].Text);
                Resources.TapAndWaitForElement(_app, c => c.Marked(homeViewProducts[i].Text), Resources.PRODUCT_VIEW_ADD_TO_BASKET);

                // Record shop view size and color
                shopViewProductTitlePrice = _app.Query(Resources.PRODUCT_VIEW_PRODUCT_TITLE_PRICE);
                shopViewProductSizeColor = _app.Query(Resources.PRODUCT_VIEW_PRODUCT_SIZE_COLOR);

                // Add product and navigate to Basket Cart View and verify navigation
                _app.Tap(Resources.PRODUCT_VIEW_ADD_TO_BASKET);
                Resources.TapAndWaitForElement(_app, Resources.ALL_VIEW_BASKET_CART, Resources.PRODUCT_VIEW_CHECKOUT);

                // Add product again and navigate to Basket Cart View and verify navigation
                _app.Back();
                Resources.WaitForElement(_app, Resources.HOME_VIEW__PRODUCTS);
                Resources.ScrollDown(_app, homeViewProducts[i].Text);
                Resources.TapAndWaitForElement(_app, c => c.Marked(homeViewProducts[i].Text), Resources.PRODUCT_VIEW_ADD_TO_BASKET);

                _app.Tap(Resources.PRODUCT_VIEW_ADD_TO_BASKET);
                Resources.TapAndWaitForElement(_app, Resources.ALL_VIEW_BASKET_CART, Resources.PRODUCT_VIEW_CHECKOUT);

                ValidateProductCartView(shopViewProductTitlePrice[0].Text, shopViewProductTitlePrice[1].Text, shopViewProductSizeColor[0].Text, shopViewProductSizeColor[1].Text, 0);
                ValidateProductCartView(shopViewProductTitlePrice[0].Text, shopViewProductTitlePrice[1].Text, shopViewProductSizeColor[0].Text, shopViewProductSizeColor[1].Text, 1);

                // Remove product from cart
                cartViewProduct = _app.Query(Resources.PRODUCT_VIEW_PRODUCTS);
                _app.DragCoordinates(cartViewProduct[0].Rect.X, cartViewProduct[0].Rect.Y, cartViewProduct[0].Rect.X + 500.0f, cartViewProduct[0].Rect.Y);

                // Verify empty basket text view is displayed
                Resources.WaitForElement(_app, Resources.PRODUCT_VIEW_CHECKOUT);

                // Remove product from cart
                cartViewProduct = _app.Query(Resources.PRODUCT_VIEW_PRODUCTS);
                _app.DragCoordinates(cartViewProduct[0].Rect.X, cartViewProduct[0].Rect.Y, cartViewProduct[0].Rect.X + 500.0f, cartViewProduct[0].Rect.Y);

                // Verify empty basket text view is displayed
                Resources.WaitForElement(_app, Resources.CART_VIEW_BASKET_EMPTY);

                Resources.TapAndWaitForElement(_app, Resources.HOME_VIEW, Resources.HOME_VIEW__PRODUCTS);
            }
        }

        /// <summary>
        /// Test case that validates each item can be checked out individually.
        /// </summary>
        [Test]
        public void ValidateSinglePurchaseAllProducts()
        {
            AppResult[] homeViewProducts = null,
                cartViewProduct = null;

            // For each product available
            Resources.WaitForElement(_app, Resources.HOME_VIEW__PRODUCTS);
            homeViewProducts = _app.Query(Resources.HOME_VIEW__PRODUCTS);
            for (int i = 0; i < homeViewProducts.Length; i += 2)
            {
                // Navigate to Product View and verify navigation
                Resources.ScrollDown(_app, homeViewProducts[i].Text);
                Resources.TapAndWaitForElement(_app, c => c.Marked(homeViewProducts[i].Text), Resources.PRODUCT_VIEW_ADD_TO_BASKET);

                // Add product and navigate to Basket Cart View and verify navigation
                _app.Tap(Resources.PRODUCT_VIEW_ADD_TO_BASKET);
                Resources.TapAndWaitForElement(_app, Resources.ALL_VIEW_BASKET_CART, Resources.PRODUCT_VIEW_CHECKOUT);

                // Navigate to Checkout View and verify navigation
                Resources.TapAndWaitForElement(_app, Resources.PRODUCT_VIEW_CHECKOUT, Resources.CHECKOUT_VIEW1_SIGN_IN);

                // Fill out password and verify information
                _app.EnterText(Resources.CHECKOUT_VIEW1_PASSWORD, Resources.TEST_ACCOUNT_PASSWORD);

                ValidateUserInfoCheckoutView1(Resources.TEST_ACCOUNT_EMAIL, Resources.TEST_ACCOUNT_PASSWORD);

                // Sign in and verify navigation
                Resources.TapAndWaitForElement(_app, Resources.CHECKOUT_VIEW1_SIGN_IN, Resources.CHECKOUT_VIEW2_PLACE_ORDER);

                // Navigate to Basket Cart View and verify navigation and remove item
                Resources.TapAndWaitForElement(_app, null, Resources.CHECKOUT_VIEW1_SIGN_IN);
                Resources.TapAndWaitForElement(_app, null, Resources.PRODUCT_VIEW_CHECKOUT);

                // Remove product from cart
                cartViewProduct = _app.Query(Resources.PRODUCT_VIEW_PRODUCTS);
                _app.DragCoordinates(cartViewProduct[0].Rect.X, cartViewProduct[0].Rect.Y, cartViewProduct[0].Rect.X + 500.0f, cartViewProduct[0].Rect.Y);

                // Verify empty basket text view is displayed
                Resources.WaitForElement(_app, Resources.CART_VIEW_BASKET_EMPTY);

                Resources.TapAndWaitForElement(_app, Resources.HOME_VIEW, Resources.HOME_VIEW__PRODUCTS);
            }
        }

        /// <summary>
        /// Test case that validates the appropriate error texts will pop up when the individual required shipping info fields are missing.
        /// </summary>
        [Test]
        public void ValidateSinglePurchaseInvalidShipping()
        {
            AppResult[] homeViewProducts = null,
                shippingInformation1 = null,
                shippingInformation2 = null;

            // For each product available
            Resources.WaitForElement(_app, Resources.HOME_VIEW__PRODUCTS);
            homeViewProducts = _app.Query(Resources.HOME_VIEW__PRODUCTS);

            // Navigate to Product View and verify navigation
            Resources.ScrollDown(_app, homeViewProducts[0].Text);
            Resources.TapAndWaitForElement(_app, c => c.Marked(homeViewProducts[0].Text), Resources.PRODUCT_VIEW_ADD_TO_BASKET);

            // Add product and navigate to Basket Cart View and verify navigation
            _app.Tap(Resources.PRODUCT_VIEW_ADD_TO_BASKET);
            Resources.TapAndWaitForElement(_app, Resources.ALL_VIEW_BASKET_CART, Resources.PRODUCT_VIEW_CHECKOUT);

            // Navigate to Checkout View and verify navigation
            Resources.TapAndWaitForElement(_app, Resources.PRODUCT_VIEW_CHECKOUT, Resources.CHECKOUT_VIEW1_SIGN_IN);

            // Fill out password and verify information
            _app.EnterText(Resources.CHECKOUT_VIEW1_PASSWORD, Resources.TEST_ACCOUNT_PASSWORD);

            // Sign in and verify navigation
            Resources.TapAndWaitForElement(_app, Resources.CHECKOUT_VIEW1_SIGN_IN, Resources.CHECKOUT_VIEW2_PLACE_ORDER);

            // Fill out valid shipping information
            shippingInformation1 = _app.Query(Resources.CHECKOUT_VIEW2_SHIPPING_INFORMATION1);
            Assert.AreEqual(Resources.TEST_SHIPPING_INFO1.Length, shippingInformation1.Length);
            for (int j = 0; j < shippingInformation1.Length; ++j)
            {
                Resources.EnterText(_app, shippingInformation1[j].Id, Resources.TEST_SHIPPING_INFO1[j]);
            }

            shippingInformation2 = _app.Query(Resources.CHECKOUT_VIEW2_SHIPPING_INFORMATION2);
            Assert.AreEqual(Resources.TEST_SHIPPING_INFO2.Length, shippingInformation2.Length);
            for (int j = 0; j < shippingInformation2.Length; ++j)
            {
                Resources.EnterText(_app, shippingInformation2[j].Id, Resources.TEST_SHIPPING_INFO2[j]);
            }

            // For each required shipping info, check if the appropriate error comes when it is missing
            shippingInformation1 = _app.Query(Resources.CHECKOUT_VIEW2_SHIPPING_INFORMATION1);
            for (int j = 0; j < shippingInformation1.Length; ++j)
            {
                ValidateMissingShippingInfoCheckoutView2(shippingInformation1[j], Resources.TEST_INVALID_SHIPPING_INFO1[j]);
            }
            shippingInformation2 = _app.Query(Resources.CHECKOUT_VIEW2_SHIPPING_INFORMATION2);
            for (int j = 0; j < shippingInformation2.Length; ++j)
            {
                ValidateMissingShippingInfoCheckoutView2(shippingInformation2[j], Resources.TEST_INVALID_SHIPPING_INFO2[j]);
            }

            Resources.TapAndWaitForElement(_app, Resources.HOME_VIEW, Resources.HOME_VIEW__PRODUCTS);
        }

        /// <summary>
        /// Test case that validates the scenario of a full successful purchase.
        /// </summary>
        [Test]
        public void ValidateSinglePurchaseValidShipping()
        {
            AppResult[] homeViewProducts = null,
                shippingInformation1 = null,
                shippingInformation2 = null;

            // For each product available
            Resources.WaitForElement(_app, Resources.HOME_VIEW__PRODUCTS);
            homeViewProducts = _app.Query(Resources.HOME_VIEW__PRODUCTS);

            // Navigate to Product View and verify navigation
            Resources.ScrollDown(_app, homeViewProducts[0].Text);
            Resources.TapAndWaitForElement(_app, c => c.Marked(homeViewProducts[0].Text), Resources.PRODUCT_VIEW_ADD_TO_BASKET);

            // Add product and navigate to Basket Cart View and verify navigation
            _app.Tap(Resources.PRODUCT_VIEW_ADD_TO_BASKET);
            Resources.TapAndWaitForElement(_app, Resources.ALL_VIEW_BASKET_CART, Resources.PRODUCT_VIEW_CHECKOUT);

            // Navigate to Checkout View and verify navigation
            _app.Tap(Resources.PRODUCT_VIEW_CHECKOUT);
            Resources.WaitForElement(_app, Resources.CHECKOUT_VIEW1_SIGN_IN);

            // Fill out password and verify information
            _app.EnterText(Resources.CHECKOUT_VIEW1_PASSWORD, Resources.TEST_ACCOUNT_PASSWORD);

            // Sign in and verify navigation
            Resources.TapAndWaitForElement(_app, Resources.CHECKOUT_VIEW1_SIGN_IN, Resources.CHECKOUT_VIEW2_PLACE_ORDER);

            // Fill out valid shipping information
            shippingInformation1 = _app.Query(Resources.CHECKOUT_VIEW2_SHIPPING_INFORMATION1);
            Assert.AreEqual(Resources.TEST_SHIPPING_INFO1.Length, shippingInformation1.Length);
            for (int j = 0; j < shippingInformation1.Length; ++j)
            {
                // Uncomment when single submission is resolved
                //                    Resources.EnterText(_app, shippingInformation1[j].Id, Resources.TEST_SHIPPING_INFO1[j]);
            }

            shippingInformation2 = _app.Query(Resources.CHECKOUT_VIEW2_SHIPPING_INFORMATION2);
            Assert.AreEqual(Resources.TEST_SHIPPING_INFO2.Length, shippingInformation2.Length);
            for (int j = 0; j < shippingInformation2.Length; ++j)
            {
                // Uncomment when single submission is resolved
                //                    Resources.EnterText(_app, shippingInformation2[j].Id, Resources.TEST_SHIPPING_INFO2[j]);
            }

            // Uncomment when single submission is resolved
            //                ValidateShippingInfoCheckoutView2(Resources.TEST_SHIPPING_INFO1, Resources.TEST_SHIPPING_INFO2);

            // Place Order and verify navigation
            Resources.TapAndWaitForElement(_app, Resources.CHECKOUT_VIEW2_PLACE_ORDER, Resources.CHECKOUT_VIEW3_ORDER_COMPLETE);
        }

        /// <summary>
        /// Method that validates the specified product details are displayed in the product shop view.
        /// </summary>
        /// <remarks>Assumes the current view is Home View.</remarks>
        /// <param name="productTitle">Title of the product.</param>
        /// <param name="productPrice">Price of the product.</param>
        private void ValidateProductShopView(string productTitle, string productPrice)
        {
            // Requires the focus to be on the Product View
            Resources.WaitForElement(_app, Resources.PRODUCT_VIEW_ADD_TO_BASKET);

            AppResult[] shopViewProductTitlePrice = null;

            // Verify product title and product price found on main page
            shopViewProductTitlePrice = _app.Query(Resources.PRODUCT_VIEW_PRODUCT_TITLE_PRICE);
            Assert.AreEqual(productTitle, shopViewProductTitlePrice[0].Text);
            Assert.AreEqual(productPrice, shopViewProductTitlePrice[1].Text);
        }

        /// <summary>
        /// Method that validates the specified product details are displayed in the product view.
        /// </summary>
        /// <remarks>Assumes the current view is Product View.</remarks>
        /// <param name="productTitle">Title of the product.</param>
        /// <param name="productPrice">Price of the product.</param>
        /// <param name="productSize">Size of the product.</param>
        /// <param name="productColor">Color of the product.</param>
        /// <param name="index">Index of the item in the cart.</param>
        private void ValidateProductCartView(string productTitle, string productPrice, string productSize, string productColor, int index)
        {
            // Requires the focus to be on the Basket Cart View
            Resources.WaitForElement(_app, Resources.PRODUCT_VIEW_CHECKOUT);

            AppResult[] cartViewProduct = null;

            // Validate product is in the cart
            cartViewProduct = _app.Query(Resources.PRODUCT_VIEW_PRODUCTS);
            Assert.AreEqual(productTitle, cartViewProduct[index * 4].Text);
            Assert.AreEqual(productPrice, cartViewProduct[index * 4 + 1].Text);
            Assert.AreEqual(productSize, cartViewProduct[index * 4 + 2].Text);
            Assert.AreEqual(productColor, cartViewProduct[index * 4 + 3].Text);
        }

        /// <summary>
        /// Method that validates the user's info in the Checkout View1.
        /// </summary>
        /// <remarks>Assumes the current view is Checkout View1.</remarks>
        /// <param name="email">Email of the user.</param>
        /// <param name="password">Password of the user.</param>
        private void ValidateUserInfoCheckoutView1(string email, string password)
        {
            // Requires the focus to be on the Checkout View1
            Resources.WaitForElement(_app, Resources.CHECKOUT_VIEW1_SIGN_IN);

            Assert.AreEqual(email, _app.Query(Resources.CHECKOUT_VIEW1_EMAIL)[0].Text);
            Assert.AreEqual(password, _app.Query(Resources.CHECKOUT_VIEW1_PASSWORD)[0].Text);    // Potential privacy issue for active APIs?
        }

        /// <summary>
        /// Method that verifies the set of shipping info are displayed in the Checkout View3.
        /// </summary>
        /// <remarks>Assumes the current view is Checkout View3.</remarks>
        /// <param name="shippingInformation1">Set of shipping info1.</param>
        /// <param name="shippingInformation2">Set of shipping info2.</param>
        private void ValidateShippingInfoCheckoutView2(string[] shippingInformation1, string[] shippingInformation2)
        {
            // Requires the focus to be on the Checkout View2
            Resources.WaitForElement(_app, Resources.CHECKOUT_VIEW2_PLACE_ORDER);

            AppResult[] shippingInformation1Results = null,
                shippingInformation2Results = null;

            shippingInformation1Results = _app.Query(Resources.CHECKOUT_VIEW2_SHIPPING_INFORMATION1);
            Assert.AreEqual(shippingInformation1.Length, shippingInformation1Results.Length);

            shippingInformation2Results = _app.Query(Resources.CHECKOUT_VIEW2_SHIPPING_INFORMATION2);
            Assert.AreEqual(shippingInformation2.Length, shippingInformation2Results.Length);

            for (int i = 0; i < shippingInformation1.Length; ++i)
            {
                Assert.AreEqual(shippingInformation1[i], shippingInformation1Results[i].Text);
            }

            for (int i = 0; i < shippingInformation2.Length; ++i)
            {
                Assert.AreEqual(shippingInformation2[i], shippingInformation2Results[i].Text);
            }
        }

        /// <summary>
        /// Method that validates the specified error query will appear when the specified shipping info is removed.
        /// </summary>
        /// <remarks>Assumes the current view is Checkout View2.</remarks>
        /// <param name="shippingInfoItem">Shipping info item to remove.</param>
        /// <param name="shippingInfoError">Error query to wait for.</param>
        private void ValidateMissingShippingInfoCheckoutView2(AppResult shippingInfoItem, Func<AppQuery, AppQuery> shippingInfoError)
        {
            if (null == shippingInfoError)
            {
                return;
            }

            // Requires the focus to be on the Checkout View2
            Resources.WaitForElement(_app, Resources.CHECKOUT_VIEW2_PLACE_ORDER);

            // Clear info line
            Resources.EnterText(_app, shippingInfoItem.Id, "");

            // Submit product and verify error
            _app.Tap(Resources.CHECKOUT_VIEW2_PLACE_ORDER);
            Resources.WaitForElement(_app, shippingInfoError);

            // Refill info line
            Resources.EnterText(_app, shippingInfoItem.Id, shippingInfoItem.Text);
            Resources.WaitForNoElement(_app, shippingInfoError);
        }
    }
}
