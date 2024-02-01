# Videos.Backend

Microservice For compression and encoding in VP09 format youtube.
Video compression in 420p, 720p, 1080p.
Creating a Thumbnail (Preview) for video.
A task queue is used in case the program crashes or an error occurs, so that it is always possible to repeat the job.

Initially I wanted to implement a queue system in rabbitmq,
but using rabbitmq as both a provider and a receiver is a bad idea + cloud storage is required. Therefore, I temporarily used hangfire, but later I need to think about a scaling plan.
