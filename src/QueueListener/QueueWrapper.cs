using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.SQS;
using Amazon.SQS.Model;

namespace QueueListener
{
    class QueueWrapper
    {
        private readonly AmazonSQSClient _sqs;

        public QueueWrapper(AmazonSQSClient sqs)
        {
            _sqs = sqs;
        }

        public async Task<string> GetQueueUrl(string queueName)
        {
            Console.WriteLine("Check if queue exist");

            var queueUrl = await CheackIfQueueExist(queueName);

            if (queueUrl == null)
            {
                Console.WriteLine("Creating new Queue");
                queueUrl = await CreateNewQueue(queueName);
            }

            return queueUrl;
        }

        public async Task<string> GetNextMessage(string queueUrl)
        {
            var receiveMessageRequest = new ReceiveMessageRequest
            {
                QueueUrl = queueUrl,
                MaxNumberOfMessages = 1
            };

            var result = string.Empty;
            do
            {
                var response = await _sqs.ReceiveMessageAsync(receiveMessageRequest);
                if (response.Messages == null || !response.Messages.Any())
                    continue;

                var message = response.Messages[0];
                result = message.Body;

                var messageRecieptHandle = message.ReceiptHandle;
                var deleteRequest = new DeleteMessageRequest { QueueUrl = queueUrl, ReceiptHandle = messageRecieptHandle };
                await _sqs.DeleteMessageAsync(deleteRequest);
            } while (result == string.Empty);

            return result;
        }

        private async Task<string> CheackIfQueueExist(string queueName)
        {
            var listQueuesRequest = new ListQueuesRequest
            {
                QueueNamePrefix = queueName
            };

            var listQueuesResponse = await _sqs.ListQueuesAsync(listQueuesRequest);

            var queueUrl = listQueuesResponse.QueueUrls.FirstOrDefault(s => s.Contains(queueName));
            return queueUrl;
        }

        private async Task<string> CreateNewQueue(string queueName)
        {
            var sqsRequest = new CreateQueueRequest
            {
                QueueName = queueName,
                Attributes = new Dictionary<string, string>
                {
                    { "ReceiveMessageWaitTimeSeconds", "20"}
                }
            };
            var createQueueResponse = await _sqs.CreateQueueAsync(sqsRequest);
            var queueUrl = createQueueResponse.QueueUrl;

            return queueUrl;
        }


    }
}