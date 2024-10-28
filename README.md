Dynamic Routing Messaging airport example for School Project.

Prerequisites:
  - RabbitMQ has to be running

What does it do?
  In this example a Producer sends 3 messages to a Dynamic Router.
  The Dynamic Router has a simple string dictionary for routes to Consumers.
  The rules are being sent by the Consumers when they start up. The rule includes a route consisting of a Guid with a key of the Consumer.

How to use:
  - Run the Dynamic Router first. It will listen to 2 channels. One for routing rules. Another for routing messages to Consumers.
  - While the Dynamic Router is running, run all of the Consumers. The console window of the Dynamic Router will show messages for each added rule/route.
  - While the Dynamic Router and the Consumers are running, run the Producer. The producer will send 3 messages to the Dynamic Router. The Dynamic Router will route the 3 messages to the Consumers. All of the messages are shown in the consoles.

Extra:
  - Try to run the Producer before some or all of the Consumers are running. The Dynamic Router will not know where to route the messages. 
