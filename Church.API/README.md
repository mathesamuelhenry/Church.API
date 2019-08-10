# Church.API

Samuels-MacBook-Pro:Church.API Samuel$ sudo lsof -i tcp:5000

COMMAND  PID   USER   FD   TYPE             DEVICE SIZE/OFF NODE NAME
dotnet  4657 Samuel  198u  IPv4 0xfedd3b53fe228eb5      0t0  TCP localhost:commplex-main (LISTEN)
dotnet  4657 Samuel  199u  IPv6 0xfedd3b53da1f3c35      0t0  TCP localhost:commplex-main (LISTEN)
Samuels-MacBook-Pro:Church.API Samuel$ 

Samuels-MacBook-Pro:Church.API Samuel$ kill -9 4657
