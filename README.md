## mongoDB
docker run -d --name app-mongoDB -v mongo-data:/data/db -p 27017:27017 -e MONGO_INITDB_ROOT_USERNAME=mongo-admin -e MONGO_INITDB_ROOT_PASSWORD=mongo-password mongo:latest

## redis
docker run --name app-redis -p 6379:6379 5002:6379 -d redis

## elasticsearch
docker network create elastic
docker pull docker.elastic.co/elasticsearch/elasticsearch:7.12.1
docker run --name es01-test --net elastic -p 9200:9200 -p 9300:9300 -e "discovery.type=single-node" -e "ELASTIC_USERNAME=elastic" -e "ELASTIC_PASSWORD=elastic" docker.elastic.co/elasticsearch/elasticsearch:7.12.1

## kibana
docker pull docker.elastic.co/kibana/kibana:7.12.1
docker run --name kib01-test --net elastic -p 5601:5601 -e "ELASTICSEARCH_HOSTS=http://es01-test:9200" docker.elastic.co/kibana/kibana:7.12.1

Stop Docker
docker stop es01-test
docker stop kib01-test

docker network rm elastic
docker rm es01-test
docker rm kib01-test