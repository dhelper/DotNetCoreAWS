using System;
using Amazon;
using Amazon.Runtime.CredentialManagement;
using Amazon.SQS;

namespace QueueListener
{
    class Program
    {
        private const string QueueName = "MyMessageQ";

        public static void Main()
        {

            var chain = new CredentialProfileStoreChain();
            if (!chain.TryGetAWSCredentials("speaker", out var awsCredentials))
            {
                Console.WriteLine("Can't find profile, configure profile and try again");

                return;
            }

            var sqs = new AmazonSQSClient(awsCredentials, RegionEndpoint.CACentral1);
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
