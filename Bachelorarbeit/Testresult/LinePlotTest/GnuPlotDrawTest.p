set terminal png
set output "Test.png"
set grid nopolar

set yrange [0:*]

set title font ",16" "RPI Manager Benchmark (500 Requests)"
set ylabel font ",16" "Total Time [ms]"
set xlabel font ",16" "Request"
set key top left reverse
plot "RPIManager0endpoint0_n200c1.data" using 9 lc rgbcolor "black" with lines title "1 Concurrent",\
	 "RPIManager0endpoint0_n200c10.data" using 9 lc rgbcolor "purple" with lines title "10 Concurrent",\
	 "RPIManager0endpoint0_n200c20.data" using 9 lc rgbcolor "green" with lines title "20 Concurrent",\
	 "RPIManager0endpoint0_n200c30.data" using 9 lc rgbcolor "orange" with lines title "30 Concurrent",\
	 "RPIManager0endpoint0_n200c50.data" using 9 lc rgbcolor "red" with lines title "50 Concurrent"

