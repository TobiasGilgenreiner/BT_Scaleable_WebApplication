set terminal png
set output "C:/Users/Tobi/Desktop/TestDest/ManagerEndpoint0/RPIManagerendpoint0/RPIManager50.png"
set grid nopolar

set yrange[0:*]

set title font ",16" "RPI Manager Benchmark (50 Requests)"
set ylabel font ",16" "Total Time [ms]"
set xlabel font ",16" "Request"
set key top left reverse
plot "C:/Users/Tobi/Desktop/TestDest/ManagerEndpoint0/RPIManagerendpoint0/RPIManager0endpoint0_n500c1.data" using 9 lc rgbcolor "black" with lines title "1 Concurrent",\
"C:/Users/Tobi/Desktop/TestDest/ManagerEndpoint0/RPIManagerendpoint0/RPIManager0endpoint0_n500c10.data" using 9 lc rgbcolor "purple" with lines title "5 Concurrent",\
"C:/Users/Tobi/Desktop/TestDest/ManagerEndpoint0/RPIManagerendpoint0/RPIManager0endpoint0_n500c20.data" using 9 lc rgbcolor "green" with lines title "10 Concurrent",