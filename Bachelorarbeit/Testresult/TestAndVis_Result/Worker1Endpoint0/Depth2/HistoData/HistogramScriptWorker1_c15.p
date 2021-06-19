#Set output
set terminal png
set output "C:/Users/Tobi/Desktop/TestDest/HistoData/HistogramWorker1_c15.png"

#Set Diagrammstyle
set style fill solid border - 1
set style data histogram
set style histogram rowstack
set boxwidth 0.8
set style histogram rowstack title offset 0,1
set grid nopolar
set grid noxtics

#GetStats
stats "C:/Users/Tobi/Desktop/TestDest/HistoData/MainDataFileWorker1_c15.data" using 4 name "A" nooutput
stats "C:/Users/Tobi/Desktop/TestDest/HistoData/MainDataFileWorker1_c15.data" using 5 name "B" nooutput
set yrange[0:(A_max + B_max) + 30]
set xtics rotate by -90

#Layout
set title font ",16" "Mean Request Time (15 Concurrent)"
set ylabel font ",16" "Time [ms]"
set xlabel font ",16" "Number of Requests n" offset 0,-2
set key top left reverse


#Plot
plot newhistogram "n = 20", \
	"C:/Users/Tobi/Desktop/TestDest/HistoData/SecondaryDataFileWorker1_n20_c15.data" using 4:xticlabels(1) lc rgbcolor "#E69F00" title "mean ctime",\
	"C:/Users/Tobi/Desktop/TestDest/HistoData/SecondaryDataFileWorker1_n20_c15.data" using 5:xticlabels(1) lc rgbcolor "#0072B2" title "mean dtime",\
newhistogram "n = 50", \
	"C:/Users/Tobi/Desktop/TestDest/HistoData/SecondaryDataFileWorker1_n50_c15.data" using 4:xticlabels(1) lc rgbcolor "#E69F00" title "",\
	"C:/Users/Tobi/Desktop/TestDest/HistoData/SecondaryDataFileWorker1_n50_c15.data" using 5:xticlabels(1) lc rgbcolor "#0072B2" title "",\
newhistogram "n = 75", \
	"C:/Users/Tobi/Desktop/TestDest/HistoData/SecondaryDataFileWorker1_n75_c15.data" using 4:xticlabels(1) lc rgbcolor "#E69F00" title "",\
	"C:/Users/Tobi/Desktop/TestDest/HistoData/SecondaryDataFileWorker1_n75_c15.data" using 5:xticlabels(1) lc rgbcolor "#0072B2" title "",\
newhistogram "n = 100", \
	"C:/Users/Tobi/Desktop/TestDest/HistoData/SecondaryDataFileWorker1_n100_c15.data" using 4:xticlabels(1) lc rgbcolor "#E69F00" title "",\
	"C:/Users/Tobi/Desktop/TestDest/HistoData/SecondaryDataFileWorker1_n100_c15.data" using 5:xticlabels(1) lc rgbcolor "#0072B2" title "",