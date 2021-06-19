set terminal png
set output "C:/Users/Tobi/Desktop/TestDest/Worker1Endpoint0/Depth3/RPI/RPIWorker120.png"
set grid nopolar

set yrange[0:*]

set title font ",16" "RPI Worker1 Benchmark (20 Requests)"
set ylabel font ",16" "Total Time [ms]"
set xlabel font ",16" "Request"
set key top left reverse
plot "C:/Users/Tobi/Desktop/TestDest/Worker1Endpoint0/Depth3/RPI/RPIWorker1endpoint0_n20c5.data" using 9 lc rgbcolor "black" with lines title "1 Concurrent",