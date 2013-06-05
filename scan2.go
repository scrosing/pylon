package main

import(
		"fmt"
		"io/ioutil"
		"flag"
"strconv"
      )

var m map[string]int64;
func get_size(root string) int64{
	var total_size int64;
	total_size = 0;
	files, _ := ioutil.ReadDir(root);
	for _, f := range files{
		if f.IsDir(){
			total_size += get_size(root + "\\" + f.Name())
		}else{
			total_size += f.Size()
		}
	}
	m[root] = total_size;
	return total_size
}


func main(){
	var root string;
	var l = 0;
	var tmp string;
	var tmp2 int64;
	var imin = 0;
	flag.Parse();
	top, _ := strconv.Atoi(flag.Arg(0));
	if top <= 0{
top = 10
}
a := make([]string, top);
b := make([]int64, top);
   for l = 0; l < top; l++{
	   b[l] = 0;
	   a[l] = "";
   }
   m = make(map[string]int64);
   root = "c:";
get_size(root);
      for i, v := range m{
	      imin = 0;
	      for l = 1; l < top; l++ {
		      if b[l] < b[imin]{
			      imin = l;
		      }
	      }
	      if b[imin] < v{
		      b[imin] = v;
		      a[imin] = i;
	      }
      }
      for n := 1; n < top; n++{
	      for l = 1; l < top; l++{
		      if (b[l] > b[l - 1]){
			      tmp = a[l];
			      a[l] = a[l - 1];
			      a[l - 1] = tmp;
			      tmp2 = b[l];
			      b[l] = b[l - 1];
			      b[l - 1] = tmp2;
		      }
	      }
      }
      for l = 0; l < top; l++ {
	      fmt.Printf("%s\t%d MB\n", a[l], m[a[l]] / 1024 / 1024)
      }
}
