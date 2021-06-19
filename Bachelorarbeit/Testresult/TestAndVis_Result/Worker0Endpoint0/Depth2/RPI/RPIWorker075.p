set terminal png
set output "C:/Users/Tobi/Desktop/TestDest/Worker0Endpoint0/Depth2/RPI/RPIWorker075.png"
set grid nopolar

set yrange[0:*]

set title font ",16" "RPI Worker0 Benchmark (75 Requests)"
set ylabel font ",16" "Total Time [ms]"
set xlabel font ",16" "Request"
set key top left reverse
plot "C:/Users/Tobi/Desktop/TestDest/Worker0Endpoint0/Depth2/RPI/RPIWorker0endpoint0_n75c1.data" using 9 lc rgbcolor "black" with lines title "1 Concurrent",\
"C:/Users/Tobi/Desktop/TestDest/Worker0Endpoint0/Depth2/RPI/RPIWorker0endpoint0_n75c15.data" using 9 lc rgbcolor "purple" with lines title "5 Concurrent",\
"C:/Users/Tobi/Desktop/TestDest/Worker0Endpoint0/Depth2/RPI/RPIWorker0endpoint0_n75c20.data" using 9 lc rgbcolor "green" with lines title "10 Concurrent",\
"C:/Users/Tobi/Desktop/TestDest/Worker0Endpoint0/Depth2/RPI/RPIWorker0endpoint0_n75c5.data" using 9 lc rgbcolor "orange" with lines title "15 Concurrent",