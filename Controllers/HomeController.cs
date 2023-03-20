using Razorpay.Api;
using System;
using System.IO;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Amazon.CloudFormation.Model;
using DocumentFormat.OpenXml.Bibliography;


namespace RazorPay.Controllers
{
    public class HomeController : Controller
    {

        public ActionResult Index()
        {
          
            return View();
        }
        public ActionResult InitiatePayment(string amount)
        {
            var key = ConfigurationManager.AppSettings["RazorPaykey"].ToString();
            var secret = ConfigurationManager.AppSettings["RazorPaySecret"].ToString();
            RazorpayClient client = new RazorpayClient(key, secret);
            Dictionary<string, object> options = new Dictionary<string, object>();
            options.Add("amount", Convert.ToDecimal(amount) * 100);
            options.Add("currency", "INR");
            Order order = client.Order.Create(options);
            ViewBag.orderId = order["id"].ToString();
            return View("Payment");
        }
       
        public ActionResult Payment(string razorpay_payment_id,string razorpay_order_id,string razorpay_signature)
        {

            String file = @"D:\Output.csv";
            Dictionary<string, string> attributes = new Dictionary<string, string>();

            attributes.Add("razorpay_payment_id", razorpay_payment_id);
            attributes.Add("razorpay_order_id", razorpay_order_id);
            attributes.Add("razorpay_signature", razorpay_signature);
           
            try
            {
                Utils.verifyPaymentSignature(attributes);
                attributes.Add("Status", "Success");
                String csv = String.Join(
   Environment.NewLine,
   attributes.Select(d => $"{d.Key};{d.Value};")
);
                System.IO.File.WriteAllText(file, csv);

                return View("PaymentSuccess");
            }
            catch(Exception ex)
            {
                attributes.Add("Status", "Failed");
                String csv = String.Join(
   Environment.NewLine,
   attributes.Select(d => $"{d.Key};{d.Value};")
);
                System.IO.File.WriteAllText(file, csv);

                return View("PaymentFailure");
            }
            return View();


        }

    }
}