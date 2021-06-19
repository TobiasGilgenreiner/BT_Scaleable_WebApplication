set terminal png
set output "C:/Users/Tobi/Desktop/TestDest/Worker0Endpoint0/Depth3/RPI/RPIWorker020.png"
set grid nopolar

set yrange[0:*]

set title font ",16" "RPI Worker0 Benchmark (20 Requests)"
set ylabel font ",16" "Total Time [ms]"
set xlabel font ",16" "Request"
set key top left reverse
plot "C:/Users/Tobi/Desktop/TestDest/Worker0Endpoint0/Depth3/RPI/RPIWorker0endpoint0_n20c1.data" using 9 lc rgbcolor "black" with lines title "1 Concurrent",