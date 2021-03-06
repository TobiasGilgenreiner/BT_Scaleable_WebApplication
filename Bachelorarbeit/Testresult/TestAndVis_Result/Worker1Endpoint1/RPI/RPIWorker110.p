set terminal png
set output "C:/Users/Tobi/Desktop/TestDest/Worker1Endpoint1/RPI/RPIWorker110.png"
set grid nopolar

set yrange[0:*]

set title font ",16" "RPI Worker1 Benchmark (10 Requests)"
set ylabel font ",16" "Total Time [ms]"
set xlabel font ",16" "Request"
set key top left reverse
plot "C:/Users/Tobi/Desktop/TestDest/Worker1Endpoint1/RPI/RPIWorker1endpoint1_n1000c1.data" using 9 lc rgbcolor "black" with lines title "1 Concurrent",\
"C:/Users/Tobi/Desktop/TestDest/Worker1Endpoint1/RPI/RPIWorker1endpoint1_n1000c10.data" using 9 lc rgbcolor "purple" with lines title "10 Concurrent",\
"C:/Users/Tobi/Desktop/TestDest/Worker1Endpoint1/RPI/RPIWorker1endpoint1_n1000c20.data" using 9 lc rgbcolor "green" with lines title "20 Concurrent",\
"C:/Users/Tobi/Desktop/TestDest/Worker1Endpoint1/RPI/RPIWorker1endpoint1_n1000c30.data" using 9 lc rgbcolor "orange" with lines title "30 Concurrent",\
"C:/Users/Tobi/Desktop/TestDest/Worker1Endpoint1/RPI/RPIWorker1endpoint1_n1000c50.data" using 9 lc rgbcolor "red" with lines title "50 Concurrent",