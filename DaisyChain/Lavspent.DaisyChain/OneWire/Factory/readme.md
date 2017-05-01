Factories and Extension on the bus allowing
code like:
```
var temp = myBus.OpenDevice<DS18B20>(address);
```
sounded like a good idea, but it work only for simple setups where a device 
is connected to a single bus, or pin, or controller. 

Cases where a device communicates on for instance a I2C-bus, and uses GPIO pins for interrupts etc. breaks this simple setup.