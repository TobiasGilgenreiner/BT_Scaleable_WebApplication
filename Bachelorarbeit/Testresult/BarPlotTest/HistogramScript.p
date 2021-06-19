#Set output
set terminal png
set output "C:\GitHubRepos\BT_Scaleable_WebApplication\Bachelorarbeit\Testresult\benchmark.png"

#Set Diagrammstyle
set style fill solid border -1
set style data histogram
set style histogram rowstack
set boxwidth 0.8
set style histogram rowstack title offset 0,1
set grid nopolar
set grid noxtics
set ytics 10

#GetStats
stats "Test.data" using 4 name "A" nooutput
stats "Test.data" using 5 name "B" nooutput
set yrange [0:(A_max + B_max) + 30]
set xtics rotate by -90

#Layout
set title font ",16" "Test mean (c Concurrent)"
set ylabel font ",16" "Time [ms]"
set xlabel font ",16" "Number of Requests n" offset 0,-2
set key top left reverse


#Plot
plot newhistogram "n = 10", \
	"Test10.data" using 4:xticlabels(1) lc rgbcolor "#E69F00" title "mean ctime",\
	"Test10.data" using 5:xticlabels(1) lc rgbcolor "#0072B2" title "mean dtime",\
	newhistogram "n = 100", \
	"Test100.data" using 4:xticlabels(1) lc rgbcolor "#E69F00" notitle,\
	"Test100.data" using 5:xticlabels(1) lc rgbcolor "#0072B2" notitle,\
	newhistogram "n = 1000", \
	"Test1000.data" using 4:xticlabels(1) lc rgbcolor "#E69F00" notitle,\
	"Test1000.data" using 5:xticlabels(1) lc rgbcolor "#0072B2" notitle