# e73-jhipster-dotnet-demo-bundle
Sample bundle containing a microservice generated using jhipster-dotnetcore adapted to work with Entando

## Blueprint for a Conference entity
The project contains a .Net 6 Blueprint-generated microservice for Entando 7.3

## Prerequisites
1. Docker account
2. Attach ent to an Entando platform (e.g. ent attach-kubeconfig config-file)
3. For local development you have to install .Net6 

## Running local version for development
1. ent bundle svc run --all
2. ent bundle run --all

## Build and publish steps  
1. ent bundle pack 
2. ent bundle publish
3. ent bundle deploy
4. ent bundle install

See https://developer.entando.com for more information.

## History
v0.0.1 First version