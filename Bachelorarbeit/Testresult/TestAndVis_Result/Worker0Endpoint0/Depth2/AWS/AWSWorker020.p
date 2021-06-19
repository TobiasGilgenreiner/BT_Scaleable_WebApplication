set terminal png
set output "C:/Users/Tobi/Desktop/TestDest/Worker0Endpoint0/Depth2/AWS/AWSWorker020.png"
set grid nopolar

set yrange[0:*]

set title font ",16" "AWS Worker0 Benchmark (20 Requests)"
set ylabel font ",16" "Total Time [ms]"
set xlabel font ",16" "Request"
set key top left reverse
plot "C:/Users/Tobi/Desktop/TestDest/Worker0Endpoint0/Depth2/AWS/AWSWorker0endpoint0_n20c10.data" using 9 lc rgbcolor "black" with lines title "1 Concurrent",\
"C:/Users/Tobi/Desktop/TestDest/Worker0Endpoint0/Depth2/AWS/AWSWorker0endpoint0_n20c15.data" using 9 lc rgbcolor "purple" with lines title "5 Concurrent",\
"C:/Users/Tobi/Desktop/TestDest/Worker0Endpoint0/Depth2/AWS/AWSWorker0endpoint0_n20c20.data" using 9 lc rgbcolor "green" with lines title "10 Concurrent",\
"C:/Users/Tobi/Desktop/TestDest/Worker0Endpoint0/Depth2/AWS/AWSWorker0endpoint0_n20c5.data" using 9 lc rgbcolor "orange" with lines title "15 Concurrent",