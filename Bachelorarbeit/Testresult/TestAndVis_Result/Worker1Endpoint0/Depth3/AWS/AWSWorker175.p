set terminal png
set output "C:/Users/Tobi/Desktop/TestDest/Worker1Endpoint0/Depth3/AWS/AWSWorker175.png"
set grid nopolar

set yrange[0:*]

set title font ",16" "AWS Worker1 Benchmark (75 Requests)"
set ylabel font ",16" "Total Time [ms]"
set xlabel font ",16" "Request"
set key top left reverse
plot "C:/Users/Tobi/Desktop/TestDest/Worker1Endpoint0/Depth3/AWS/AWSWorker1endpoint0_n75c1.data" using 9 lc rgbcolor "black" with lines title "1 Concurrent",\
"C:/Users/Tobi/Desktop/TestDest/Worker1Endpoint0/Depth3/AWS/AWSWorker1endpoint0_n75c10.data" using 9 lc rgbcolor "purple" with lines title "5 Concurrent",\
"C:/Users/Tobi/Desktop/TestDest/Worker1Endpoint0/Depth3/AWS/AWSWorker1endpoint0_n75c15.data" using 9 lc rgbcolor "green" with lines title "10 Concurrent",\
"C:/Users/Tobi/Desktop/TestDest/Worker1Endpoint0/Depth3/AWS/AWSWorker1endpoint0_n75c5.data" using 9 lc rgbcolor "orange" with lines title "15 Concurrent",