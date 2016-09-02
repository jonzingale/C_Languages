#include <iostream>
#include <string>
using namespace std;

unsigned long long primes_siv(unsigned long long l, bool show_all) {
  unsigned long long lim = l + 1;
  unsigned long long lim_2 = (l - 1) / 2;
  unsigned long long current_prime = 0;
  unsigned long long max_prime = 0;
  unsigned long long i, j, c, ilim;

  bool * composites;
  composites = (bool*) malloc (lim_2 + 1);

  for (i = 1; i <= lim_2; ++i) {
    composites[i] = false;
  }
  ilim = (lim - 1) / 3;
  for (i = 1; i < ilim; ++i) {
    for (j = 1; j <= j; ++j) {
      c = (2 * i * j + i + j);
      if (c <= lim_2) {
        composites[c] = true;
      } else {
        break;
      }
    }
  }
  for (unsigned long long i = lim_2; i > 1; --i) {
    if (!composites[i]) {
      current_prime = i * 2 + 1;
      if (max_prime == 0) {
        max_prime = current_prime;
      }
      if (show_all) {
        cout << current_prime << endl;
      } else {
        return current_prime;
      }
    }
  }

  if (show_all) {
    cout << "3" << endl;
    cout << "2" << endl;
  } else {
    cout << "max prime: " << max_prime << endl;
  }

  return max_prime;
} 


int main(int argc, char *argv[]) {
  unsigned long long lim;
  unsigned long long max_prime;
  bool show_all;

  if ( argc < 2 ) // argc should be 2 for correct execution
    // We print argv[0] assuming it is the program name
    cout<<"usage: "<< argv[0] <<" <limit> [show_all]\n";
  else {
    lim = stoull(argv[1]);
    show_all = (argc > 2 && strcmp(argv[2], "show_all") == 0);
    max_prime = primes_siv(lim, show_all);
  }
  return 0;
}
