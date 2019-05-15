//#define RUN_LOCAL 

using System;
using Amazon;
using Amazon.Runtime;
using Amazon.Runtime.CredentialManagement;
using Amazon.SQS;

namespace QueueListener
{
    class Program
    {
        private const string QueueName = "MyMessageQ";

        public static void Main()
        {
#if RUN_LOCAL
            var config = new AmazonSQSConfig();
            config.ServiceURL = "http://127.0.0.1:4100";
            var sqs = new AmazonSQSClient(config);
#else
            var chain = new CredentialProfileStoreChain();
            // TODO: fill your credentials!
            if (!chain.TryGetAWSCredentials("DevDaysEurope2019", out var awsCredentials))
            {
                Console.WriteLine("Can't find profile, configure profile and try again");

                return;
            }
            
            var sqs = new AmazonSQSClient(awsCredentials, RegionEndpoint.EUCentral1);
#endif
            var wrapper = new QueueWrapper(sqs);

            var queueUrl = wrapper.GetQueueUrl(QueueName).Result;
            Console.WriteLine($"Listening for messages, queue url: {queueUrl}");

            do
            {
                var message = wrapper.GetNextMessage(queueUrl).Result;

                Console.WriteLine(message);
                Console.WriteLine("-----------------");

            } while (true);
        }
    }
}
