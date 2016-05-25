## Obtaining your Publishable API Keys

Your Publishable API Keys can be found by logging into your [Stripe Dashboard](https://dashboard.stripe.com/).  You will need to create an account first if you do not already have one.

Once you are logged into the dashboard, navigate to the ***API Keys*** tab, inside of the ***Account Settings*** menu.


## Configuring your app
You'll need to configure Stripe in your app with your publishable API key. We recommend doing this in your `AppDelegate`'s `DidFinishLaunching` method on iOS, or in your `MainActivity` or `Application.OnCreate` on Android so that it'll be set for the entire lifecycle of your app.

**iOS**
```csharp
// AppDelegate.cs
public override bool FinishedLaunching (UIApplication app, NSDictionary options)
{
	Stripe.StripeClient.DefaultPublishableKey = "pk_test_1234-YOUR-KEY-HERE";
	
	// ... your other code
}
```

**Android**
```csharp
protected override void OnCreate (Bundle bundle)
{
	base.OnCreate (bundle);

	Stripe.StripeClient.DefaultPublishableKey = "pk_test_1234-YOUR-KEY-HERE";
	
	// ... your other code
}
```


### Test Mode:
When you're using your test publishable key, our libraries give you the ability to test your payment flow without having to charge real credit cards.

**Note about Pay on iOS**:
On iOS8+, our Apple Pay integration presents an alternate credit card selection UI that produces test cards.  If you want to use this alternative Test UI, you will need to implement `IStripeTestPaymentAuthorizationViewControllerDelegate` and call `CreateTestToken` to initiate the 'test' flow.  This has been designed to work the same way as the actual Pay flow.  Check out the `iOS Sample` to see how this test flow is implemented.

If you're building your own form or using `StripeView`, using the card number `4242424242424242` (along with any CVC and future expiration date) in the `CreateToken` call will return a `Token` which when used on your server will result in successful transactions.

You'll need to swap it out with your live publishable key when you are ready to publish to production. You can see all your API keys in your dashboard.




## Collecting Credit Card Information
At some point in the flow of your app, you'll want to obtain payment details from the user. There are two ways to do this:

 1. Use Apple's Apple Pay framework to access your users' stored payment information
 2. Use the `StripeView` UI Control to capture credit card number, expiry date, and CVC (available on both iOS and Android)
 3. Build your own credit card form from scratch
 
Apple Pay only supports certain US credit cards on the latest iOS devices. We recommend using Apple Pay in combination with option 2 or 3 as a fallback on devices where Apple Pay is not available.




### 1. Using Apple Pay on iOS 8+
With Apple's Apple Pay, you'll be able to access payment information stored on your customers' iOS devices.

To use Pay, you'll need to generate a `PKPaymentRequest` to submit to Apple. We've provided a convenience method to generate one, which you can customize as you see fit. For a more in-depth reference, see the `PKPaymentRequest` documentation.

After you create the request, query the device to see if Pay is available (i.e. if your app is running on the latest hardware and the user has added a valid credit card):

```csharp
var paymentRequest = StripeClient.PaymentRequest (
						"YOUR.MERCHANT.ID",
						new NSDecimalNumber ("10.00"),
						"USD",
						"Premium llama food");

if (StripeClient.CanSubmitPaymentRequest (paymentRequest)) {
	// ...	
} else {
	// Show the user your own credit card form to capture input
}				
```

`YOUR.MERCHANT.ID` is an identifier that you obtain directly from Apple—log in to your account to do so. Next, you should create and display the payment request view controller.

```csharp
if (StripeClient.CanSubmitPaymentRequest (paymentRequest) {
	
	var paymentController = StripeClient.PaymentController (
  								paymentRequest,
  								this);
  								
	PresentViewController (paymentController, true, null);	
} else {
	// Show the user your own credit card form
}
```

You may have noticed that `PaymentController (..)` needs to be given a `IPKPaymentAuthorizationViewControllerDelegate`.  You'll handle the `PKPayment` that the controller returns by implementing this interface. In this case, we'll implement it in our ViewController:

```csharp
public void DidAuthorizePayment (
	PKPaymentAuthorizationViewController controller, 
	PKPayment payment, 		
	PKPaymentAuthorizationHandler completion)
{
	// We'll implement this method below in the Getting Started
	// section 'Creating a single-use token'.
	// Note that we've also been given a callback that takes a
	// `PKPaymentAuthorizationStatus`. We'll call this function with either
	// `PKPaymentAuthorizationStatus.Success` or `PKPaymentAuthorizationStatus.Failure`
	// after all of our asynchronous code is finished executing. This is how the
	// `PKPaymentAuthorizationViewController` knows when and how to update its UI.
    
	this.HandlePaymentAuthorization (payment, completion);
}

public void PaymentAuthorizationViewControllerDidFinish (PKPaymentAuthorizationViewController controller)
{
	this.DismissViewController (true, null);
}
```          

After the controller has returned with a `PKPayment`, we can move ahead to the section 'Creating Tokens'.





### 2. Using StripeView

To make the process of collecting Credit Card information easier, you can use a pre-made UI control called `StripeView`.

This view captures the following credit card information:
 - Credit Card Number
 - Expiry Date
 - CVC
 
Using the control is as simple as using any other View on a given platform.  When you're ready to capture Credit Card data, simply access the `StripeView.Card` property which will return an instance of `Card`.

**iOS**
```csharp
// Create the View and add it to your parent
stripeView = new StripeView ();
stripeView.Frame = new CGRect (10, 100, stripeView.Frame.Width, stripeView.Frame.Height);
View.AddSubview (stripeView);
```

**Android**
```xml
<Stripe.StripeView
        android:id="@+id/stripeView"
        android:layout_width="fill_parent"
        android:layout_height="wrap_content"
        android:layout_marginBottom="10dp" />
```

When paying, you can access the `Card` property of your view to be used in the `CreateToken` call:

```csharp
// When paying...
var card = stripeView.Card;
StripeClient.CreateToken (card);
```


### 3. Building your own form

If you build your own payment form, you'll need to collect at least your customers' card numbers and expiration dates. You should likely also collect the CVC to prevent fraud. You can also optionally collect the user's name and billing address for additional fraud protection.







## Creating Tokens

Our libraries shoulder the burden of PCI compliance by helping you avoid the need to send card data directly to your server. Instead, our libraries send credit card data directly to our servers, where we can convert them to tokens. You can charge these tokens later in your server-side code.



### Using a PKPayment (from Apple Pay)

After you've received a PKPayment, you can turn it into a single-use Stripe token with a simple method call:

```csharp
async Task HandlePaymentAuthorization (PKPayment payment, PKPaymentAuthorizationHandler completion)
{
	try {
		var token = await StripeClient.CreateToken (payment);
		
		// Submit the new token info to your Credit card server
		// CreateBackendCharge should call the Pay completion action
		// when it's done!
		await CreateBackendCharge (token, completion);
		
	} catch (Exception ex) {
		// Call the Pay completion handler
		completion (PKPaymentAuthorizationStatus.Failure);
		
		Console.WriteLine (ex);
	}
}
```		

### Using a Card

If you're using your own form, you can assemble the data into an `Card`. Once you've collected the card number, expiration, and CVC, package them up in an `Card` object and invoke the `StripeClient.CreateToken` method, instructing the library to send off the credit card data to Stripe and return a token.

```csharp
public async Task Save () 
{
	var card = new Card {
		Number = "4242424242424242",
		ExpiryMonth = 12,
		ExpiryYear = 16,
		CVC = 123
	};
	
	try {
		var token = await StripeClient.CreateToken (card);, (token, error);
		
		// Slightly different for non-Apple Pay use, see 
		// 'Sending the token to your server' for more info
		await CreateBackendCharge (token);
		
	} catch (Exception ex) {

		// Handle a failure
		Console.WriteLine (ex);
	}			
}
```

In the example above, we're calling `CreateToken` when a save button is tapped. The important thing to ensure is the `CreateToken` isn't called before the user has finished inputting their details.

Handling error messages and showing activity indicators while we're creating the token is up to you.






## Sending the token to your server

The completion handler you gave to `CreateToken` will be called whenever Stripe returns with a token (or error). You'll need to send the token off to your server so you can, for example, charge the card.

Here's how it looks for a token created with Apple Pay:

```csharp
async Task CreateBackendCharge (Token token, PKPaymentAuthorizationHandler completion) 
{
	var http = new HttpClient ();
	
	var content = new StringContent ("StripeTokenId=" + token.TokenId);

	var response = await http.PostAsync("https://example.com/token", content);

	if (response.StatusCode != HttpStatusCode.OK)
		completion (PKPaymentAuthorizationStatus.Failure);
	else
		completion (PKPaymentAuthorizationStatus.Success);
}		
```

If you're not using Apple Pay, the above code would be mostly the same, though you'll want a slightly different signature—the completion callback shown above is Pay specific. You would instead want to implement custom error and success handling.

On the server, you just need to implement an endpoint that will accept a parameter called `StripeTokenId`.  Make sure any communication with your server is SSL secured to prevent eavesdropping.

Once you have a Stripe `Token` representing a card on our server we can go ahead and charge it, save it for charging later, or sign the user up for a subscription. 

Take a look at the full example application to see everything put together.

