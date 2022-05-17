import { Component, Input, OnInit, Self } from '@angular/core';
import { ControlValueAccessor, NgControl } from '@angular/forms';

@Component({
  selector: 'app-text-input',
  templateUrl: './text-input.component.html',
  styleUrls: ['./text-input.component.css']
})
 export class TextInputComponent implements ControlValueAccessor  { 
   // ControlValueAccessor - Defines an interface that acts as a bridge between the Angular forms API and a native element in the DOM.

  @Input() label!: string;
  @Input() type = 'text';

  constructor(@Self() public ngControl: NgControl) { 
    //Parameter decorator to be used on constructor parameters, which tells the DI framework to start dependency resolution from the local injector.
    //Resolution works upward through the injector hierarchy, so the children of this class must configure their own providers or be prepared for a null result

    //after injecting NgControl ,now we have acces to formcontrol
    this.ngControl.valueAccessor = this;
  }
//below methods will executte one by one
  writeValue(obj: any): void {
  }

  registerOnChange(fn: any): void {
  }

  registerOnTouched(fn: any): void {
  }

}
